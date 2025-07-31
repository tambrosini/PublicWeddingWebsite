# Wedding Website

A comprehensive wedding website application built with Angular and .NET 8, designed to manage wedding invitations, RSVPs, and guest information seamlessly.

![Wedding Website Screenshot](documentation/placeholder.png)

## 🌟 Features

### Guest Experience

- **RSVP System**: Guests can easily respond to invitations using unique invite codes
- **Dietary Requirements**: Capture special dietary needs and allergies
- **Wedding Information**: Access to venue details, accommodation recommendations, and event timeline
- **FAQ Section**: Comprehensive answers to common wedding questions
- **Responsive Design**: Beautiful experience across desktop, tablet, and mobile devices

### Admin Management

- **Admin Dashboard**: Real-time overview of guest statistics and recent activity
- **Guest Management**: Create, edit, view, and delete guest records
- **Invite Management**: Create and manage invitations with unique codes
- **RSVP Tracking**: Monitor response status and completion rates
- **Excel Export**: Download comprehensive guest lists with attendance and dietary information
- **Email Notifications**: Automated notifications when RSVPs are submitted

### Security & Authentication

- **Guest Authentication**: Secure access using unique invite codes
- **Admin Authentication**: Protected admin area with user management
- **Authorization Guards**: Route protection for both guest and admin areas

## 🛠️ Technology Stack

### Frontend

- **Angular 19** with Server-Side Rendering (SSR)
- **Bootstrap 5** with responsive design
- **FontAwesome** for icons
- **TypeScript** for type safety
- **SCSS** for styling

### Backend

- **.NET 8** Web API
- **Entity Framework Core** with migrations
- **ASP.NET Core Identity** for authentication
- **Swagger/OpenAPI** for API documentation
- **ClosedXML** for Excel export functionality
- **[Mailjet](https://www.mailjet.com/)** for email notifications

### Database Support

- **PostgreSQL** (Primary)
- **SQL Server** (Alternative)

### DevOps & CI/CD

- **Azure Pipelines** for build and deployment
- **Docker** containerization support
- **YAML** pipeline configurations for Azure deployments

## 📂 Project Structure

```text
PublicWeddingWebsite/
├── WeddingWebsite-Angular/          # Angular frontend application
│   ├── src/app/
│   │   ├── auth/                    # Admin authentication & management
│   │   ├── guest-login/             # Guest authentication
│   │   ├── rsvp/                    # RSVP functionality
│   │   ├── faq/                     # FAQ component
│   │   ├── info/                    # Wedding information
│   │   └── services/                # API services
│   └── public/assets/               # Static assets & content
├── WeddingWebsite-Backend/          # .NET 8 Web API
│   ├── WeddingInvites/
│   │   ├── Controllers/             # API controllers
│   │   ├── Domain/                  # Data models
│   │   ├── Services/                # Business logic
│   │   ├── Database/                # Entity Framework context
│   │   └── Security/                # Authentication logic
│   └── WeddingInvites.Test/         # Unit tests
├── devops/                          # CI/CD pipeline configurations
│   ├── PostgreSQL/                  # PostgreSQL deployment scripts
│   └── AzureSql/                    # Azure SQL deployment scripts
└── documentation/                   # Project documentation
```

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ and npm
- .NET 8 SDK
- PostgreSQL or SQL Server
- Visual Studio Code or preferred IDE

### Docker Deployment (Recommended)

The easiest way to run the application is using Docker, which bundles everything into a single container including the Angular frontend, .NET backend, and PostgreSQL database.

#### Build the Container

```bash
docker build -t wedding-website:latest .
```

#### Run the Application

```bash
# Run in background (detached mode)
docker run -d -p 8080:80 --name wedding-test wedding-website:latest

# Or run in foreground to see logs
docker run -p 8080:80 wedding-website:latest
```

The application will be available at `http://localhost:8080`

#### Container Features

- **Complete Stack**: Angular frontend, .NET 8 backend, and PostgreSQL database in one container
- **Automatic Setup**: Database initialization and Entity Framework migrations run automatically
- **Production Ready**: Optimized build with proper authentication and CORS configuration
- **Volume Support**: Use Docker Compose for persistent data storage

#### Docker Management Commands

```bash
# View running containers
docker ps

# View container logs
docker logs wedding-test

# Stop the container
docker stop wedding-test

# Remove the container
docker rm wedding-test

# Access container shell (for debugging)
docker exec -it wedding-test /bin/bash
```

### Manual Development Setup

For local development without Docker:

### Database Setup

1. **Choose your database provider** by updating `appsettings.json`:

   ```json
   {
     "DatabaseType": "postgres", // or "sqlserver"
     "ConnectionStrings": {
       "PostgresConnection": "Host=localhost;Port=5432;Database=WeddingInvites;Username=postgres;Password=YourPassword",
       "DefaultConnection": "Data Source=localhost;Initial Catalog=WeddingInvites;..."
     }
   }
   ```

2. **Run database migrations**:

   ```bash
   cd WeddingWebsite-Backend/WeddingInvites
   dotnet ef database update
   ```

### Backend Setup

1. Navigate to the backend directory:

   ```bash
   cd WeddingWebsite-Backend/WeddingInvites
   ```

2. Restore packages and run:

   ```bash
   dotnet restore
   dotnet run
   ```

The API will be available at `https://localhost:7001` or `http://localhost:5000`

### Frontend Setup

1. Navigate to the frontend directory:

   ```bash
   cd WeddingWebsite-Angular
   ```

2. Install dependencies:

   ```bash
   npm install
   ```

3. Start the development server:

   ```bash
   npm start
   ```

The application will be available at `http://localhost:4200`

## 🔧 Configuration

### Email Configuration

Configure email notifications in `appsettings.json`:

```json
{
  "Email": {
    "ApiKey": "your-mailjet-api-key",
    "ApiSecret": "your-mailjet-secret",
    "To": "admin@example.com"
  }
}
```

### Admin User Setup

Set up the initial admin user:

```json
{
  "SeedAdminUser": true,
  "AdminUser": {
    "UserName": "admin@example.com",
    "Password": "SecurePassword123!"
  }
}
```

## 🎯 Key Workflows

### Guest RSVP Process

1. Guest receives invitation with unique code
2. Guest visits website and enters invite code
3. System displays guest details and RSVP form
4. Guest selects attendance and enters dietary requirements
5. System saves response and sends confirmation email
6. Admin receives notification of RSVP submission

### Administrative Tasks

1. Admin logs into secure dashboard
2. View real-time statistics and recent activity
3. Manage guests and invitations
4. Export comprehensive reports
5. Handle RSVP responses and updates

## 📊 Features Screenshots

> **Note:** Replace placeholder images with actual screenshots when available

- Dashboard Overview: ![Dashboard](documentation/placeholder.png)
- RSVP Interface: ![RSVP](documentation/placeholder.png)
- Guest Management: ![Guests](documentation/placeholder.png)
- Mobile Experience: ![Mobile](documentation/placeholder.png)

## 🧪 Testing

Run backend tests:

```bash
cd WeddingWebsite-Backend/WeddingInvites.Test
dotnet test
```

Run frontend tests:

```bash
cd WeddingWebsite-Angular
npm test
```

## 🚀 Deployment

### Docker Containerized Deployment (Recommended)

The simplest deployment method is using the Docker container which includes the complete application stack:

```bash
# Build the container
docker build -t wedding-website:latest .

# Deploy using Docker Compose (recommended for production)
docker run -d -p 8080:80 --name wedding-app wedding-website:latest
```

**Production Considerations:**

- Use Docker Compose with volumes for data persistence
- Configure reverse proxy (nginx) for HTTPS
- Set up monitoring and logging
- Use Docker secrets for sensitive configuration

### Azure DevOps Pipelines

The project also includes Azure DevOps pipeline configurations for automated deployment:

- **PostgreSQL**: `devops/PostgreSQL/pipeline-postgres.yaml`
- **Azure SQL**: `devops/AzureSql/pipeline.yaml`

### Manual Production Build

For traditional deployment without Docker:

Build the application for production:

```bash
# Frontend
cd WeddingWebsite-Angular
npm run build:ssr

# Backend
cd WeddingWebsite-Backend/WeddingInvites
dotnet publish -c Release -o ./publish
```

### Environment Variables

Ensure the following environment variables are configured for production:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__PostgresConnection` or `ConnectionStrings__DefaultConnection`
- `Email__ApiKey` and `Email__ApiSecret` (Mailjet credentials)
- `AdminUser__UserName` and `AdminUser__Password`

## 📖 API Documentation

The API includes Swagger documentation available at:

- **Development**: `https://localhost:7001/swagger`
- **Production**: `{your-domain}/swagger`

### Key API Endpoints

| Endpoint | Method | Description | Auth Required |
|----------|--------|-------------|---------------|
| `/api/rsvp/get-invite` | POST | Get invite by code | No |
| `/api/rsvp/update-invite` | POST | Submit RSVP | No |
| `/api/guest` | GET/POST/PUT/DELETE | Guest management | Admin |
| `/api/invite` | GET/POST/PUT/DELETE | Invite management | Admin |
| `/api/rsvp/download` | GET | Download RSVP report | Admin |

## 🔧 Advanced Configuration

### CORS Configuration

Update `appsettings.json` for production domains:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ]
  }
}
```

### Database Connection Strings

**PostgreSQL (Recommended for production):**

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=your-db-host;Port=5432;Database=WeddingInvites;Username=your-user;Password=your-password;SSL Mode=Require;"
  }
}
```

**SQL Server:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=WeddingInvites;User Id=your-user;Password=your-password;Encrypt=true;TrustServerCertificate=false;"
  }
}
```

### Email Configuration (Mailjet)

1. Sign up at [Mailjet](https://www.mailjet.com/)
2. Get your API Key and Secret from the dashboard
3. Configure in `appsettings.json`:

```json
{
  "Email": {
    "ApiKey": "your-mailjet-api-key",
    "ApiSecret": "your-mailjet-secret",
    "To": "admin@yourdomain.com"
  }
}
```

## 🔒 Security Considerations

- **HTTPS**: Always use HTTPS in production
- **Secrets Management**: Use Azure Key Vault or similar for sensitive configuration
- **Database**: Use strong passwords and restrict network access
- **CORS**: Configure allowed origins restrictively
- **Admin Account**: Use strong passwords and consider 2FA implementation

## 🐳 Docker Support

The application includes Docker support. Build and run with:

```bash
# Build backend image
cd WeddingWebsite-Backend
docker build -t wedding-api .

# Build frontend image  
cd WeddingWebsite-Angular
docker build -t wedding-ui .

# Run with docker-compose
docker-compose up -d
```

## 📊 Monitoring & Logging

### Application Insights (Azure)

Configure Application Insights for production monitoring:

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-instrumentation-key"
  }
}
```

### Health Checks

The API includes health check endpoints:

- `/health` - Basic health check
- `/health/ready` - Readiness check (database connectivity)

## 🔍 Troubleshooting

### Common Issues

**Database Connection Errors:**

- Verify connection string format
- Check firewall rules for database server
- Ensure database exists and migrations are applied

**CORS Errors:**

- Verify frontend URL is in `Cors.AllowedOrigins`
- Check for trailing slashes in URLs

**Email Not Sending:**

- Verify Mailjet API credentials
- Check spam folders
- Review application logs for email service errors

**Build Failures:**

- Ensure .NET 8 SDK is installed
- Check Node.js version (18+)
- Clear npm cache: `npm cache clean --force`

### Logs Location

- **Development**: Console output
- **Production**: Application Insights or configured logging provider
- **Docker**: `docker logs container-name`

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Follow the existing code style and patterns
4. Add tests for new functionality
5. Update documentation as needed
6. Submit a pull request with a clear description

### Development Standards

- Follow C# coding conventions
- Use TypeScript strict mode
- Write unit tests for business logic
- Include integration tests for API endpoints
- Document complex business rules

## 📋 Production Checklist

Before deploying to production:

- [ ] Update all default passwords
- [ ] Configure proper CORS origins
- [ ] Set up SSL certificates
- [ ] Configure email service
- [ ] Set up monitoring and logging
- [ ] Test backup and recovery procedures
- [ ] Review security configurations
- [ ] Load test with expected traffic

## 📝 License

This project is personal and not currently licensed for redistribution.

## 🎉 Wedding Day Ready

This application provides everything needed to manage your wedding invitations digitally, from initial setup through the big day. The responsive design ensures guests can RSVP from any device, while the comprehensive admin interface keeps you organized and informed throughout the planning process.

For support or questions, please create an issue in the repository or contact the development team.
