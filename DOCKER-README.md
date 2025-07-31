# Wedding Website - Containerized Setup

This document describes how to run the wedding website application in a single Docker container.

## Architecture

The containerized application includes:
- **Angular Frontend** (Node.js 22.13) - Built and served as static files
- **.NET 8.0 Backend** - API and web server
- **PostgreSQL Database** - Data storage
- **Entity Framework Migrations** - Database schema management

All components run in a single container for simplified deployment.

## Quick Start

### Option 1: Using the build script
```bash
./build-container.sh
```

### Option 2: Using Docker directly
```bash
# Build the image
docker build -t wedding-website:latest .

# Run the container
docker run -p 8080:80 wedding-website:latest
```

### Option 3: Using Docker Compose
```bash
docker-compose up --build
```

## Access the Application

Once running, the application will be available at:
- **Web Application**: http://localhost:8080

## Build Process

The Docker build follows these steps:

1. **Frontend Build Stage**:
   - Uses Node.js 22.13 Alpine image
   - Installs npm dependencies with `--legacy-peer-deps`
   - Builds Angular application using `npm run build`
   - Outputs to `dist/wedding-website/browser/`

2. **Backend Build Stage**:
   - Uses .NET 8.0 SDK image
   - Restores NuGet packages
   - Copies built frontend to `wwwroot/`
   - Publishes .NET application

3. **Runtime Stage**:
   - Uses .NET 8.0 ASP.NET runtime image
   - Installs PostgreSQL server
   - Installs Entity Framework CLI tools
   - Sets up startup script for initialization

## Database Setup

The container automatically:
1. Initializes PostgreSQL if not already done
2. Creates the `WeddingInvites` database
3. Creates a `postgres` user with password `Password01!`
4. Runs Entity Framework migrations
5. Starts the web application

## Configuration

The application is configured for production use with:
- PostgreSQL as the database backend
- CORS enabled for the application domain
- Static file serving for the Angular frontend
- All necessary environment variables set

## Environment Variables

Key environment variables used:
- `ASPNETCORE_ENVIRONMENT=Production`
- `DatabaseType=postgres`
- `ConnectionStrings__PostgresConnection=Host=localhost;Port=5432;Database=WeddingInvites;Username=postgres;Password=Password01!`

## Volumes

The Docker Compose setup includes a named volume for PostgreSQL data persistence:
- `postgres_data` - Stores database files

## Security Notes

- The default PostgreSQL password is `Password01!` - change this for production use
- The container runs with sudo privileges to manage PostgreSQL - consider security implications
- CORS is configured for localhost - update for production domains

## Troubleshooting

### Container fails to start
Check the logs:
```bash
docker logs <container-id>
```

### Database connection issues
Ensure PostgreSQL service is running within the container. The startup script should handle this automatically.

### Frontend not loading
Verify that the Angular build completed successfully and files are in the `wwwroot` directory.

## Production Considerations

For production deployment:
1. Change default passwords
2. Configure proper CORS origins
3. Set up SSL/TLS termination
4. Consider using external PostgreSQL database
5. Implement proper logging and monitoring
6. Use container orchestration (Kubernetes, Docker Swarm) for scalability
