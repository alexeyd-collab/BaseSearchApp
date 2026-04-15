#!/usr/bin/env bash
# =============================================================================
# install.sh — One-shot production install for SearchApp on Ubuntu 22.04/24.04
#
# Usage:
#   1. Copy the project to the server (e.g. via scp or git clone)
#   2. sudo bash deploy/install.sh [/path/to/project]
#
# What it does:
#   - Installs .NET 10 SDK, Node.js 20 LTS, Redis, and Nginx
#   - Creates a dedicated 'searchapp' system user
#   - Sets up the app directory at /var/www/searchapp
#   - Installs and enables the systemd service
#   - Installs and enables the Nginx site
#   - Publishes the application
# =============================================================================
set -euo pipefail

# ---- Resolve project root -----------------------------------------------
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="${1:-$(dirname "$SCRIPT_DIR")}"

if [[ $EUID -ne 0 ]]; then
    echo "ERROR: This script must be run as root (sudo)." >&2
    exit 1
fi

echo ""
echo "========================================="
echo "  SearchApp — Production Install"
echo "  Project: $PROJECT_ROOT"
echo "========================================="
echo ""

# ---- Helper: wait for apt locks -----------------------------------------
wait_for_apt() {
    local WAIT=0
    local MAX=120  # seconds
    while fuser /var/lib/dpkg/lock-frontend /var/lib/apt/lists/lock &>/dev/null 2>&1; do
        if [[ $WAIT -eq 0 ]]; then
            echo "==> Waiting for apt lock to be released (another process is running apt)..."
        fi
        sleep 5
        WAIT=$((WAIT + 5))
        if [[ $WAIT -ge $MAX ]]; then
            echo "ERROR: apt lock not released after ${MAX}s. Aborting." >&2
            exit 1
        fi
    done
}

# ---- Install system dependencies ----------------------------------------
echo "==> Updating package lists..."
wait_for_apt
apt-get update -y

echo "==> Installing prerequisites..."
wait_for_apt
apt-get install -y curl gnupg apt-transport-https software-properties-common

# .NET 10
if ! command -v dotnet &>/dev/null || ! dotnet --list-sdks | grep -q "^10\."; then
    echo "==> Installing .NET 10 SDK..."
    # Official Microsoft package feed
    curl -fsSL https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb \
        -o /tmp/packages-microsoft-prod.deb
    dpkg -i /tmp/packages-microsoft-prod.deb
    wait_for_apt
    apt-get update -y
    wait_for_apt
    apt-get install -y dotnet-sdk-10.0
else
    echo "==> .NET 10 SDK already installed."
fi

# Node.js 20 LTS (via NodeSource)
if ! command -v node &>/dev/null || [[ "$(node -v)" != v20* ]]; then
    echo "==> Installing Node.js 20 LTS..."
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash -
    wait_for_apt
    apt-get install -y nodejs
else
    echo "==> Node.js already installed: $(node -v)"
fi

# Redis
echo "==> Installing Redis..."
wait_for_apt
apt-get install -y redis-server
systemctl enable --now redis-server
echo "==> Redis status:"
redis-cli ping

# Nginx
echo "==> Installing Nginx..."
wait_for_apt
apt-get install -y nginx
systemctl enable nginx

# ---- Create service user ------------------------------------------------
if ! id -u searchapp &>/dev/null; then
    echo "==> Creating 'searchapp' system user..."
    useradd --system --no-create-home --shell /usr/sbin/nologin searchapp
else
    echo "==> 'searchapp' user already exists."
fi

# ---- Prepare app directory ----------------------------------------------
APP_DIR="/var/www/searchapp"
echo "==> Creating app directory: $APP_DIR"
mkdir -p "$APP_DIR"
chown searchapp:searchapp "$APP_DIR"

# ---- Build & publish ----------------------------------------------------
echo "==> Running publish script..."
export SUDO_USER="${SUDO_USER:-}"
bash "$SCRIPT_DIR/publish.sh"

# ---- Install systemd service --------------------------------------------
echo "==> Installing systemd service..."
cp "$SCRIPT_DIR/searchapp.service" /etc/systemd/system/searchapp.service
systemctl daemon-reload
systemctl enable searchapp
systemctl restart searchapp
echo "==> Service status:"
systemctl status searchapp --no-pager

# ---- Install Nginx site -------------------------------------------------
echo "==> Configuring Nginx..."
cp "$SCRIPT_DIR/nginx.conf" /etc/nginx/sites-available/searchapp
ln -sf /etc/nginx/sites-available/searchapp /etc/nginx/sites-enabled/searchapp
# Remove default site if still enabled
rm -f /etc/nginx/sites-enabled/default
nginx -t
systemctl reload nginx

# ---- Firewall (ufw) -----------------------------------------------------
if command -v ufw &>/dev/null; then
    echo "==> Configuring UFW firewall..."
    ufw allow OpenSSH
    ufw allow 'Nginx Full'
    ufw --force enable
fi

# ---- Done ---------------------------------------------------------------
echo ""
echo "========================================="
echo "  ✓ SearchApp installation complete!"
echo ""
echo "  Service:  systemctl status searchapp"
echo "  Logs:     journalctl -u searchapp -f"
echo "  Redeploy: sudo bash $SCRIPT_DIR/publish.sh"
echo ""
echo "  Next steps:"
echo "    1. Edit /etc/systemd/system/searchapp.service to set"
echo "       the Redis connection string if different from localhost:6379"
echo "    2. Set up HTTPS with: sudo certbot --nginx"
echo "========================================="
