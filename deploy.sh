#!/bin/bash
set -e

echo "===> Stopping service"
sudo systemctl stop magazynek.service

echo "===> Building app (Release)"
dotnet publish -c Release -o /home/osmc/Magazynek/publish

echo "===> Starting service"
sudo systemctl start magazynek.service

echo "===> Done!"