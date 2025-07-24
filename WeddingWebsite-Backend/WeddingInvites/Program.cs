using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using WeddingInvites.Database;
using WeddingInvites.Security;
using WeddingInvites.Services;

var builder = WebApplication.CreateBuilder(args);

// Explicitly load environment-specific config
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

ConfigurationManager configuration = builder.Configuration;

builder.Services.Configure<SeedUserOptions>(configuration.GetSection("AdminUser"));


// Entity Framework

var databaseType = configuration.GetSection("DatabaseType").Value!;

if (databaseType == "sqlserver")
{
    Console.WriteLine("Using SQL Server database");
    var connectionString = configuration.GetConnectionString("DefaultConnection")!;
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else if (databaseType == "postgres")
{
    Console.WriteLine("Using Postgres database");
    var connectionString = configuration.GetConnectionString("PostgresConnection")!;
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}

//Identity
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true; // Optional but recommended
});


builder.Services.AddAuthorizationBuilder();

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<RsvpService>();
builder.Services.AddScoped<GuestService>();
builder.Services.AddScoped<InviteService>();
builder.Services.AddScoped<EventLogService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

var allowRegistration = builder.Configuration.GetValue<bool>("AllowRegistration");

app.MapCustomIdentityApi<IdentityUser>(allowRegistration);

// Seed user if "AllowRegistration" is true
var seedAdminUser = builder.Configuration.GetValue<bool>("SeedAdminUser");
if (seedAdminUser)
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;

        // Get the UserManager and SeedUserOptions
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var seedUserOptions = serviceProvider.GetRequiredService<IOptions<SeedUserOptions>>().Value;

        // Check if the user already exists
        var user = await userManager.FindByEmailAsync(seedUserOptions.UserName);
        if (user == null)
        {
            // Create the user
            var newUser = new IdentityUser
            {
                UserName = seedUserOptions.UserName,
                Email = seedUserOptions.UserName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, seedUserOptions.Password);
            if (result.Succeeded)
            {
                Console.WriteLine("Seed user created successfully.");
            }
            else
            {
                Console.WriteLine("Failed to create seed user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("Seed user already exists.");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DynamicCorsPolicy");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure static files middleware only in non-testing environments
if (!app.Environment.IsEnvironment("Testing"))
{
    // Basic static files middleware for wwwroot
    app.UseStaticFiles();
    
    // Angular app static files
    string browserPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "browser");
    if (Directory.Exists(browserPath))
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(browserPath),
            RequestPath = ""
        });
    }
    else
    {
        Console.WriteLine($"Warning: Angular app directory not found at {browserPath}");
    }
    
    // Only apply the SPA fallback for non-API routes
    app.MapWhen(
        context => !context.Request.Path.StartsWithSegments("/api") && 
                   !context.Request.Path.Value.Contains("."),
        appBuilder => {
            appBuilder.UseStaticFiles();
            appBuilder.UseRouting();
            appBuilder.UseEndpoints(endpoints => {
                endpoints.MapFallbackToFile("browser/index.html");
            });
        }
    );
}

app.Run();

public partial class Program
{}