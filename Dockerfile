# Multi-stage Dockerfile for Wedding Website
# Stage 1: Build Angular Frontend
FROM node:22.13-alpine AS frontend-builder

WORKDIR /app/frontend

# Copy package files
COPY WeddingWebsite-Angular/package*.json ./

# Install dependencies (following build-ui.sh approach)
RUN npm i --legacy-peer-deps

# Copy source code
COPY WeddingWebsite-Angular/ ./

# Build the application (Angular outputs to dist/wedding-website by default)
RUN npm run build

# Stage 2: Build .NET Backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder

WORKDIR /app/backend

# Copy project files
COPY WeddingWebsite-Backend/WeddingInvites/*.csproj ./
COPY WeddingWebsite-Backend/WeddingInvites/*.sln ./

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY WeddingWebsite-Backend/WeddingInvites/ ./

# Create wwwroot directory and copy built frontend to the correct subdirectory
COPY --from=frontend-builder /app/frontend/dist/wedding-website/browser/ ./wwwroot/browser/

# Install Entity Framework CLI tool in SDK stage
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Build the application
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Final runtime image with PostgreSQL
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Install PostgreSQL and .NET SDK (needed for EF migrations)
RUN apt-get update && apt-get install -y \
    postgresql \
    postgresql-contrib \
    sudo \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Install .NET SDK for EF CLI
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0 \
    && rm -rf /var/lib/apt/lists/*

# Install Entity Framework CLI tool
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app

# Copy published application
COPY --from=backend-builder /app/publish .

# Copy the original project files needed for migrations
COPY --from=backend-builder /app/backend/ ./source/

# Create startup script
RUN cat > /app/startup.sh << 'EOF'
#!/bin/bash
set -e

echo "Starting PostgreSQL..."

# Find PostgreSQL version and set paths dynamically
PG_VERSION=$(ls /usr/lib/postgresql/ | head -n1)
PG_BIN_DIR="/usr/lib/postgresql/${PG_VERSION}/bin"
PG_DATA_DIR="/var/lib/postgresql/${PG_VERSION}/main"
PG_CONFIG_DIR="/etc/postgresql/${PG_VERSION}/main"

echo "Found PostgreSQL version: $PG_VERSION"
echo "Using data directory: $PG_DATA_DIR"

# Initialize PostgreSQL if not already done
if [ ! -d "$PG_DATA_DIR" ]; then
    echo "Initializing PostgreSQL database..."
    mkdir -p "$PG_DATA_DIR"
    chown postgres:postgres "$PG_DATA_DIR"
    sudo -u postgres "$PG_BIN_DIR/initdb" -D "$PG_DATA_DIR"
fi

# Configure PostgreSQL for password authentication
echo "Configuring PostgreSQL authentication..."
cat > "$PG_CONFIG_DIR/pg_hba.conf" << 'HBAEOF'
# TYPE  DATABASE        USER            ADDRESS                 METHOD
local   all             postgres                                trust
local   all             all                                     trust
host    all             all             127.0.0.1/32            md5
host    all             all             ::1/128                 md5
HBAEOF

# Start PostgreSQL service
echo "Starting PostgreSQL service..."
sudo service postgresql start

# Wait a bit for PostgreSQL to start
sleep 3

# Set postgres user password
echo "Setting up database and user..."
sudo -u postgres psql -c "ALTER USER postgres WITH PASSWORD 'Password01!';"
sudo -u postgres psql -c "CREATE DATABASE \"WeddingInvites\" OWNER postgres;" 2>/dev/null || true

# Reload PostgreSQL configuration to apply authentication changes
sudo service postgresql reload

# Wait for PostgreSQL to be ready
echo "Waiting for PostgreSQL to be ready..."
until sudo -u postgres psql -c '\q' 2>/dev/null; do
  >&2 echo "PostgreSQL is unavailable - sleeping"
  sleep 1
done

echo "PostgreSQL is ready!"

# Set environment variables for the application
export DatabaseType="postgres"
export ConnectionStrings__PostgresConnection="Host=localhost;Port=5432;Database=WeddingInvites;Username=postgres;Password=Password01!"

# Run migrations
echo "Running database migrations..."
cd /app/source
dotnet ef database update --connection "Host=localhost;Port=5432;Database=WeddingInvites;Username=postgres;Password=Password01!"

# Start the application
echo "Starting the application..."
cd /app
exec dotnet WeddingInvites.dll
EOF

RUN chmod +x /app/startup.sh

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV DatabaseType=postgres
ENV ConnectionStrings__PostgresConnection="Host=localhost;Port=5432;Database=WeddingInvites;Username=postgres;Password=Password01!"

# Create non-root user for running the app
RUN useradd -m -s /bin/bash appuser && \
    usermod -aG sudo appuser && \
    echo 'appuser ALL=(ALL) NOPASSWD: ALL' >> /etc/sudoers

# Expose port
EXPOSE 80

# Start with the startup script
CMD ["/app/startup.sh"]
