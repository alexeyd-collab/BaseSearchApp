# SearchApp

A full-stack web application for searching and exploring GitHub repositories. 
This project combines a performant .NET 10 backend with a modern React frontend (powered by Vite).

## Features

- **GitHub Repository Search**: Fast and responsive search utilizing the GitHub API.
- **Robust Telemetry & Logging**: Integrated with OpenTelemetry and Serilog for comprehensive observability.
- **Flexible Caching**: Pluggable storage provider — use In-Memory for development, Redis for production.
- **Modern Frontend**: Built with React 18 and Vite for rapid development and optimized builds.

## Tech Stack

### Backend
- **Framework**: .NET 10 / C#
- **Logging**: Serilog (Async File sink) + OpenTelemetry
- **Observability**: OpenTelemetry (AspNetCore & Http instrumentation, Console exporter)
- **Caching**: StackExchange.Redis (production) / In-Memory (development)

### Frontend
- **Framework**: React 18
- **Build Tool**: Vite
- **Linting**: ESLint

## Project Structure

```
SearchApp/
├── deploy/                       # Production deployment scripts
│   ├── install.sh                #   One-shot Ubuntu server installer
│   ├── publish.sh                #   Build & publish script
│   ├── searchapp.service         #   systemd unit file
│   └── nginx.conf                #   Nginx reverse proxy config
├── frontend/                     # React/Vite Single Page Application
├── Constants/                    # Shared backend constants
├── Controllers/                  # API endpoint controllers
├── Extensions/                   # Dependency Injection & Middleware setup
├── Models/                       # DTOs and Domain Models
├── Services/                     # Business logic & external API integrations
│   └── Storage/                  #   Pluggable caching backends
├── appsettings.json              # Base configuration
├── appsettings.Production.json   # Production overrides (Redis enabled)
└── Program.cs                    # .NET application entry point
```

## Getting Started (Development)

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js v20 LTS](https://nodejs.org/)
- Git

### Backend

```bash
git clone <repository_url>
cd SearchApp
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5000`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The dev server will be available at `http://localhost:5173` and proxies API calls to the backend.

---

## Configuration

All settings live in `appsettings.json` and can be overridden per environment.

### StorageSettings

| Key | Type | Default | Description |
|-----|------|---------|-------------|
| `Provider` | `Memory` \| `DistributedCache` | `Memory` | Cache backend to use |
| `RedisConnectionString` | string | _(none)_ | Required when `Provider = DistributedCache` |
| `CacheExpirationMinutes` | int | `10` | How long results are cached |

**Development** (`appsettings.json`):
```json
"StorageSettings": {
  "Provider": "Memory"
}
```

**Production** (`appsettings.Production.json`):
```json
"StorageSettings": {
  "Provider": "DistributedCache",
  "RedisConnectionString": "localhost:6379",
  "CacheExpirationMinutes": 30
}
```

> You can also override the connection string at runtime using the environment variable:  
> `StorageSettings__RedisConnectionString=redis-host:6379`

### GitHubApiSettings

| Key | Default | Description |
|-----|---------|-------------|
| `BaseUrl` | `https://api.github.com/search/repositories?q=` | GitHub search endpoint |
| `UserAgent` | `SearchApp` | User-Agent header sent to GitHub |
| `DefaultAcceptHeader` | `application/json` | Accept header |

---

## Production Deployment (Ubuntu 22.04 / 24.04)

### Architecture

```
Internet → Nginx (port 80/443) → Kestrel (port 5000) → Redis (port 6379)
```

The app runs as a dedicated `searchapp` system user under `systemd`, with Nginx as the reverse proxy.

### One-Shot Install (Fresh Server)

Copy the project to the server, then:

```bash
sudo bash deploy/install.sh
```

This will automatically:
1. Install .NET 10 SDK, Node.js 20, Redis, and Nginx
2. Create the `searchapp` system user
3. Build the React frontend and publish the .NET app to `/var/www/searchapp`
4. Install and enable the `searchapp` systemd service
5. Configure Nginx as a reverse proxy
6. Enable the UFW firewall (SSH + HTTP/HTTPS)

### Redeployment (Updates)

```bash
sudo bash deploy/publish.sh
```

### Useful Commands

```bash
# Check service status
sudo systemctl status searchapp

# Stream live logs
sudo journalctl -u searchapp -f

# Restart the service
sudo systemctl restart searchapp

# Test Nginx config
sudo nginx -t && sudo systemctl reload nginx
```

### HTTPS with Let's Encrypt

After installation, secure the site with a free TLS certificate:

```bash
sudo apt install certbot python3-certbot-nginx -y
sudo certbot --nginx -d yourdomain.com
```

---

## Logging

Application logs are written to:
- **File**: `Logs/searchapp-log-<date>.txt` (rotated daily)
- **Console / journald**: via Serilog + OpenTelemetry

On the production server, stream logs with:
```bash
journalctl -u searchapp -f
```

