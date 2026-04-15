#!/usr/bin/env bash
# =============================================================================
# publish.sh — Build and publish SearchApp to /var/www/searchapp
# Run as: sudo bash deploy/publish.sh
# =============================================================================
set -euo pipefail

APP_DIR="/var/www/searchapp"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Determine the real (non-root) user to run npm as.
# When invoked via sudo, $SUDO_USER is the original user.
# If run directly as root (e.g. on a server), fall back to root.
NPM_USER="${SUDO_USER:-root}"

echo "==> Building React frontend (as user: $NPM_USER)..."
cd "$PROJECT_ROOT/frontend"

# Use existing node_modules if present (avoids network fetch on re-deploy).
# Run install/build as the non-root user so npm uses their cache & config.
if [ -d "node_modules" ]; then
    echo "    node_modules already present — skipping install."
else
    echo "    Installing npm dependencies..."
    sudo -u "$NPM_USER" npm install
fi

sudo -u "$NPM_USER" npm run build

echo "==> Copying frontend dist to wwwroot..."
mkdir -p "$PROJECT_ROOT/wwwroot"
rm -rf "$PROJECT_ROOT/wwwroot/"*
cp -r dist/* "$PROJECT_ROOT/wwwroot/"

echo "==> Publishing .NET application..."
cd "$PROJECT_ROOT"
dotnet publish SearchApp.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained false \
  --output "$APP_DIR"

echo "==> Setting permissions..."
chown -R searchapp:searchapp "$APP_DIR"

echo "==> Restarting service..."
systemctl restart searchapp

echo ""
echo "✓ SearchApp published to $APP_DIR"
