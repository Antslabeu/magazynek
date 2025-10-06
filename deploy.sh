#!/bin/bash
set -e

echo "===> Stopping service"
sudo systemctl stop magazynek.service

echo "===> Building app (Release)"
cd /home/osmc/magazynek
dotnet publish -c Release -o /home/osmc/magazynek/publish

echo "===> Starting service"
sudo systemctl start magazynek.service

echo "===> Done!"
