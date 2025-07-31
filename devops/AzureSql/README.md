# Azure SQL Pipeline Configuration

Azure DevOps pipeline for deploying the Wedding Website application to Azure App Service with Azure SQL Database.

## Pipeline Files

- `pipeline.yaml` - Main pipeline
- `build.yaml` - Build and test with SQL Server container
- `deploy.yaml` - Deploy to Azure App Service with SQL Database migrations

## Required Pipeline Variables

Configure these variables in Azure DevOps:

### Required Variables
- `AppServiceSubscription` - Azure service connection name
- `WebAppName` - Target Azure App Service name
- `ServerName` - Azure SQL Server name
- `DatabaseName` - Azure SQL Database name
- `SqlUsername` - SQL Server username
- `SqlPassword` - SQL Server password
- `APP_URL` - Application base URL (for Angular API configuration)

## Deployment Process

1. **Build Stage**: 
   - Tests against SQL Server container
   - Generates SQL migration scripts
   - Builds and packages application

2. **Deploy Stage**:
   - Applies database migrations using SqlAzureDacpacDeployment task
   - Deploys application to Azure App Service
   - Configures application settings automatically

## Database Migrations

The pipeline uses Azure DevOps SqlAzureDacpacDeployment task to apply migrations to Azure SQL Database. This provides built-in rollback capabilities and integration with Azure networking.

## Notes

- Uses SQL Server 2019 container for testing
- Automatically configures App Service connection strings
- Requires Azure SQL Database firewall rules to allow Azure services
