# PostgreSQL Pipeline Configuration

Azure DevOps pipeline for deploying the Wedding Website application to Azure App Service with PostgreSQL/Supabase database.

## Pipeline Files

- `pipeline-postgres.yaml` - Main pipeline
- `build-postgres.yaml` - Build and test with PostgreSQL container  
- `deploy-postgres.yaml` - Deploy to Azure App Service with automatic PostgreSQL migrations

## Required Pipeline Variables

Configure these variables in Azure DevOps:

### Required Variables
- `POSTGRES_CONNECTION_STRING` - Supabase connection string
- `AZURE_SUBSCRIPTION` - Azure service connection name
- `WEB_APP_NAME` - Target Azure App Service name
- `APP_URL` - Application base URL (for Angular API configuration)

### Connection String Format
```
Host=db.yourproject.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your_password;SSL Mode=Require;Trust Server Certificate=true
```

## Deployment Process

1. **Build Stage**: Tests against PostgreSQL container and generates migration scripts
2. **Deploy Stage**: 
   - Automatically runs database migrations using `psql` 
   - Deploys application code to Azure App Service
   - Application settings are managed manually (not deployed via pipeline)

## Database Migrations

The pipeline automatically runs PostgreSQL migrations during deployment using the generated SQL script. Migrations will fail the deployment if they encounter errors.

**Note**: Ensure your Supabase connection string allows connections from Azure DevOps build agents.
