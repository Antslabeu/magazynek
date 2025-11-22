#!/usr/bin/env bash
set -euo pipefail

#BRANCH="feature/identity-system"
BRANCH="main"

APP_DIR="/home/zuczek/dotnet/magazynek"
PUBLISH_DIR="$APP_DIR/publish"
SERVICE_NAME="magazynek.service"

trap 'echo "❌ Deploy failed"; exit 1' ERR

echo "=== [1/5] Update z Git ==="
cd "$APP_DIR"

# stash lokalnych zmian (także untracked)
if ! git diff --quiet || ! git diff --cached --quiet || [ -n "$(git ls-files --others --exclude-standard)" ]; then
  git stash push -u -m "deploy-$(date -Iseconds)" || true
fi

git fetch --all --tags --prune
git checkout -B "$BRANCH" "origin/$BRANCH"
git submodule update --init --recursive --jobs 4

GIT_SHA="$(git rev-parse --short HEAD)"
echo "   -> branch: $BRANCH @ $GIT_SHA"

echo "=== [2/5] Czyszczenie starych buildów ==="
rm -rf "$APP_DIR/bin" "$APP_DIR/obj" "$PUBLISH_DIR"

echo "=== [3/5] Kompilacja (linux-arm64, self-contained) ==="
dotnet publish -c Release -r linux-arm64 --self-contained true \
  -p:PublishTrimmed=false \
  -p:ContinuousIntegrationBuild=true \
  -p:SourceRevisionId="$GIT_SHA" \
  -o "$PUBLISH_DIR"

echo "=== [4/5] Uprawnienia ==="
chmod +x "$PUBLISH_DIR/Magazynek"

echo "=== [5/5] Restart systemd ==="
sudo systemctl daemon-reload
sudo systemctl restart "$SERVICE_NAME"
sudo systemctl status "$SERVICE_NAME" --no-pager -l

echo "✅ Deploy OK: $BRANCH @ $GIT_SHA"