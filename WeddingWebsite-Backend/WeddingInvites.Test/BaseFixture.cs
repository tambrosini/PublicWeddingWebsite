using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeddingInvites.Database;
using WeddingInvites.Domain;
using WeddingInvites.Services;

namespace WeddingInvites.Test;

public class BaseFixture<T> : WebApplicationFactory<Program>
where T : BaseEntity
{
    private HttpClient WebClient { get; set; }

    public TestDataManager<T> TestDataManager { get; set; }

    private static List<int> PersistedEntityIds { get; set; }

    private bool _disposed = false;

    public string ConnectionString { get; set; }

    public string Endpoint = "api/" + typeof(T).Name.ToLower();

    protected internal Func<T?, Task> Execute { get; set; } = async (model) => await Task.CompletedTask;

    public BaseFixture()
    {
        WebClient = CreateClient();

        Cleanup = async () =>
        {
            // This ensures we have a fresh context before cleanup operations
            await TestDataManager.GenerateNewDbContext();
            await TestDataManager.Cleanup(PersistedEntityIds);
            PersistedEntityIds.Clear();
        };
        
        Setup = async () => 
        {
            // Create a fresh context for each test setup
            await TestDataManager.GenerateNewDbContext();
            await Task.CompletedTask;
        };
        TestDataManager = new TestDataManager<T>(ConnectionString);

        PersistedEntityIds = new List<int>();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Configure environment to use Testing
        builder.UseEnvironment("Testing");
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // This ensures appsettings.Testing.json is loaded after appsettings.json
            // and overrides any matching values
            var projectDir = Directory.GetCurrentDirectory();
            config.AddJsonFile(Path.Combine(projectDir, "appsettings.Testing.json"), 
                optional: false, 
                reloadOnChange: true);
        });
        
        builder.ConfigureServices(services =>
        {
            // Get the updated configuration to use for the connection string
            var sp = services.BuildServiceProvider();
            var configuration = sp.GetRequiredService<IConfiguration>();
            ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
            
            // Replace IEmailService with TestEmailService for testing
            var emailServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IEmailService));
                
            if (emailServiceDescriptor != null)
            {
                services.Remove(emailServiceDescriptor);
            }
            
            services.AddScoped<IEmailService, TestEmailService>();
            
            // Your existing authentication configuration
            var authServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IAuthenticationService));
                    
            if (authServiceDescriptor != null)
            {
                services.Remove(authServiceDescriptor);
            }
            
            // Configure auth for testing
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", options => { });
            
            // Remove existing DbContext registration and add one with test connection string
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ApplicationDbContext));
                
            if (descriptor != null)
            {
                services.Remove(descriptor);
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(ConnectionString));
            }
        });
    }

    protected internal Func<Task> Cleanup { get; set; }
    
    protected internal new Func<Task> Setup { get; set; }

    public async Task RunTest()
    {
        await TestDataManager.GenerateNewDbContext();
        await Setup();

        try
        {
            await Execute(null);
        }
        finally
        {
            await Cleanup();
        }
    }
    public void SetAuthCookie(string username = "test-user@email.com", string firstName = "John", string lastName = "Wick")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, username),
            new Claim(ClaimTypes.GivenName, firstName),
            new Claim(ClaimTypes.Name, $"{firstName} {lastName}"),
            new Claim(ClaimTypes.Surname, lastName),
        };
    
        var identity = new ClaimsIdentity(claims, "mockAuthType");
        var principal = new ClaimsPrincipal(identity);
    
        var authTicket = new AuthenticationTicket(principal, "mockAuthScheme");
    
        // Extract the necessary parts of the auth ticket
        var ticketData = new
        {
            // Store the authentication scheme
            Scheme = authTicket.AuthenticationScheme,
            // Store the user identifier - typically a name identifier claim
            UserId = authTicket.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
            // Store all the claims as simple key-value pairs
            Claims = authTicket.Principal.Claims.Select(c => new { c.Type, c.Value }).ToList(),
            // Store any ticket properties you need
            Properties = authTicket.Properties?.Items,
            // Store expiration if needed
            Expires = authTicket.Properties?.ExpiresUtc
        };
    
        // Serialize this simplified data structure instead
        var cookieValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ticketData)));
        
        string authCookieName = ".AspNetCore.Identity.Application"; // Check your actual cookie name
        WebClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={cookieValue}");
    }

    private readonly JsonSerializerSettings QueryStringSerializerSettings = new JsonSerializerSettings()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateFormatString = "s"
    };

    public string BuildQueryStringForObject(object request, string seperator = ",")
    {
        if (request == null)
            throw new ArgumentNullException("request");

        var json = JsonConvert.SerializeObject(request, QueryStringSerializerSettings);

        var jObj = (JObject)JsonConvert.DeserializeObject(json, QueryStringSerializerSettings);

        var paramPairs = new List<KeyValuePair<string, string>>();

        foreach (var item in jObj.Children().Cast<JProperty>().Where(jp => jp.Value.Type == JTokenType.Array))
        {
            paramPairs.AddRange(item.Value.Children().Select(val => new KeyValuePair<string, string>(Uri.EscapeDataString(item.Name), Uri.EscapeDataString(val.Type == JTokenType.Date ? val.ToObject<DateTime>().ToString("s", CultureInfo.InvariantCulture) : val.ToString()))));
        }

        foreach (var item in jObj.Children().Cast<JProperty>()
                     .Where(jp => jp.Value.Type != JTokenType.Array && !string.IsNullOrEmpty(jp.Value.ToString())))
        {
            paramPairs.Add(new KeyValuePair<string, string>(Uri.EscapeDataString(item.Name), Uri.EscapeDataString(item.Value.Type == JTokenType.Date ? item.Value.ToObject<DateTime>().ToString("s", CultureInfo.InvariantCulture) : item.Value.ToString())));
        }

        var qs = string.Join("&", paramPairs.Select(k => k.Key + "=" + k.Value));
        return qs;
    }

    private void AddAuthCookie(HttpRequestMessage request)
    {
        if (WebClient.DefaultRequestHeaders.Contains("Cookie"))
        {
            request.Headers.Add("Cookie", WebClient.DefaultRequestHeaders.GetValues("Cookie"));
        }
    }

    public async Task TearUpAsync(T model)
    {
        await TestDataManager.GenerateNewDbContext();
        await TestDataManager.CreateEntityAsync(model);
    }
    
    public async Task TearUpAsync(List<T> modelList)
    {
        await TestDataManager.GenerateNewDbContext();
        await TestDataManager.CreateManyAsync(modelList);
    }
    
    public async Task<HttpResponseMessage> ExecuteRequest(HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new Uri($"{endpoint}", UriKind.Relative),
            Version = HttpVersion.Version11
        };

        request.Headers.Add("Accept", "application/json");
        AddAuthCookie(request);

        return await WebClient.SendAsync(request);
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        return await DeleteAsync(id);
    }

    public async Task<TEntity?> Exists<TEntity>(int id)
    {
        var response = await ExecuteRequest(HttpMethod.Get, $"{Endpoint}/exists/{id}");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<TEntity>(await response.Content.ReadAsStringAsync());
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await GetAsync<T>($"{id}");
    }

    public async Task<List<TEntity>> ListAsync<TEntity>(string url = "")
    {
        return await GetAsync<List<TEntity>>(url);
    }

    public async Task<T> CreateAsync(T data)
    {
        var response = await PostAsync(data);
        
        PersistedEntityIds.Add(response.Id);

        return response;
    }
    
    public async Task<TReturnEntity> CreateAsync<TSendEntity, TReturnEntity>(TSendEntity data)
        where TReturnEntity : BaseEntity
    {
        var response = await PostAsync<TSendEntity, TReturnEntity>(data);
        
        PersistedEntityIds.Add(response.Id);

        return response;
    }
    
    public async Task<T> UpdateAsync(T data)
    {
        return await PutAsync(data);
    }
    
    public async Task<TEntity> GetAsync<TEntity>(string uri)
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{Endpoint}/{uri}", UriKind.Relative),
            Version = HttpVersion.Version11
        };

        request.Headers.Add("Accept", "application/json");
        AddAuthCookie(request);

        var response = await WebClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<TEntity>(await response.Content.ReadAsStringAsync())!;
    }
    
    public async Task<T> PostAsync(T data)
    {
        var serializedContent = JsonConvert.SerializeObject(data);

        var content = new StringContent(serializedContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{Endpoint}", UriKind.Relative),
            Version = HttpVersion.Version11,
            Content = content
        };

        request.Headers.Add("Accept", "application/json");
        AddAuthCookie(request);

        var response = await WebClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;
    }
    
    public async Task<T> PutAsync(T data)
    {
        var serializedContent = JsonConvert.SerializeObject(data);

        var content = new StringContent(serializedContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{Endpoint}/{data.Id}", UriKind.Relative),
            Version = HttpVersion.Version11,
            Content = content
        };

        request.Headers.Add("Accept", "application/json");
        AddAuthCookie(request);

        var response = await WebClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // For NoContent responses, return the original data with updated properties
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return data;
        }

        // If the endpoint returns content, deserialize it
        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{Endpoint}/{id}", UriKind.Relative),
            Version = HttpVersion.Version11
        };

        request.Headers.Add("Accept", "application/json");
        AddAuthCookie(request);

        var response = await WebClient.SendAsync(request);

        // Return true only if deletion was successful (204 No Content)
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return true;
        }

        // Optionally handle not found or other statuses
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        // Throw on unexpected status codes
        response.EnsureSuccessStatusCode();

        return true;
    }

    /// <summary>
    /// Custom update, we can specify the send and return type
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="id"></param>
    /// <param name="url"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TReturnEntity"></typeparam>
    /// <returns></returns>
    public async Task<TReturnEntity> UpdateAsync<TEntity, TReturnEntity>(TEntity entity, int id, string url = "")
    {
        // Serialize the entity
        var serializedContent = JsonConvert.SerializeObject(entity);
        var content = new StringContent(serializedContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        // Build the URI
        string requestUri = $"{Endpoint}";

        // Add custom URL if provided
        if (!string.IsNullOrEmpty(url))
        {
            // Ensure URL doesn't start with a slash if we're appending
            if (url.StartsWith("/"))
                url = url.Substring(1);
            
            requestUri += $"/{url}";
        }

        // Add the ID at the end
        requestUri += $"/{id}";

        // Create and configure the request
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(requestUri, UriKind.Relative),
            Version = HttpVersion.Version11,
            Content = content
        };

        request.Headers.Add("Accept", "application/json");
        AddAuthCookie(request);

        // Send the request
        var response = await WebClient.SendAsync(request);
        
        // Read the response content
        string responseContent = await response.Content.ReadAsStringAsync();
        
        // Check if the response is successful
        if (!response.IsSuccessStatusCode)
        {
            // Handle error responses
            if (response.StatusCode == HttpStatusCode.BadRequest) // 400 response
            {
                // Log or handle the error details
                Console.WriteLine($"Bad Request: {responseContent}");
                // You can also try to deserialize the error response into a model
                // var errorDetails = JsonConvert.DeserializeObject<ErrorModel>(responseContent);
                
                // Optionally throw a custom exception with error details
                throw new HttpRequestException($"Bad Request (400): {responseContent}", null, response.StatusCode);
            }
            
            // For other error status codes
            response.EnsureSuccessStatusCode(); // This will throw for other error codes
        }

        // For NoContent responses, create default instance or try to cast entity to TReturnEntity
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            // If TEntity and TReturnEntity are the same type, try to cast
            if (typeof(TEntity) == typeof(TReturnEntity) && entity is TReturnEntity returnEntity)
            {
                return returnEntity;
            }
        
            // Otherwise, create default instance
            return default(TReturnEntity)!;
        }

        // If the endpoint returns content, deserialize and return it
        return JsonConvert.DeserializeObject<TReturnEntity>(responseContent)!;
    }
    
    /// <summary>
    /// Custom post, we can specify the send and return type
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="url"></param>
    /// <typeparam name="TSendEntity"></typeparam>
    /// <typeparam name="TReturnEntity"></typeparam>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<TReturnEntity> PostAsync<TSendEntity, TReturnEntity>(TSendEntity entity, string url = "")
    {
        // Serialize the entity to JSON
        var serializedContent = JsonConvert.SerializeObject(entity);
        var content = new StringContent(serializedContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        // Build the URI
        string requestUri = $"{Endpoint}";

        // Add custom URL if provided
        if (!string.IsNullOrEmpty(url))
        {
            // Ensure URL doesn't start with a slash if we're appending
            if (url.StartsWith("/"))
                url = url.Substring(1);
            
            requestUri += $"/{url}";
        }

        // Create and configure the request
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUri, UriKind.Relative),
            Version = HttpVersion.Version11,
            Content = content
        };

        // Set JSON Accept header
        request.Headers.Add("Accept", MediaTypeNames.Application.Json);
        AddAuthCookie(request);

        // Send the request
        var response = await WebClient.SendAsync(request);
        
        // Read the response content
        string responseContent = await response.Content.ReadAsStringAsync();
        
        // Check if the response is successful
        if (!response.IsSuccessStatusCode)
        {
            // Handle error responses
            if (response.StatusCode == HttpStatusCode.BadRequest) // 400 response
            {
                // Log or handle the error details
                Console.WriteLine($"Bad Request: {responseContent}");
                // You can also try to deserialize the error response into a model
                // var errorDetails = JsonConvert.DeserializeObject<ErrorModel>(responseContent);
                
                // Optionally throw a custom exception with error details
                throw new HttpRequestException($"Bad Request (400): {responseContent}", null, response.StatusCode);
            }
            
            // For other error status codes
            response.EnsureSuccessStatusCode(); // This will throw for other error codes
        }

        // For NoContent responses, return default instance
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default(TReturnEntity)!;
        }

        // Deserialize JSON response
        return JsonConvert.DeserializeObject<TReturnEntity>(responseContent)!;
    }
    
    
    /// <summary>
    /// Custom post, we can specify the send type but no return data is expected (empty Ok response)
    /// </summary>
    /// <param name="entity">The entity to send</param>
    /// <param name="url">Optional URL to append to the endpoint</param>
    /// <typeparam name="TSendEntity">Type of entity being sent</typeparam>
    /// <returns>True if the request was successful</returns>
    /// <exception cref="HttpRequestException">Thrown when the request fails</exception>
    public async Task<bool> PostAsync<TSendEntity>(TSendEntity entity, string url = "")
    {
        // Serialize the entity to JSON
        var serializedContent = JsonConvert.SerializeObject(entity);
        var content = new StringContent(serializedContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        // Build the URI
        string requestUri = $"{Endpoint}";

        // Add custom URL if provided
        if (!string.IsNullOrEmpty(url))
        {
            // Ensure URL doesn't start with a slash if we're appending
            if (url.StartsWith("/"))
                url = url.Substring(1);
            
            requestUri += $"/{url}";
        }

        // Create and configure the request
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUri, UriKind.Relative),
            Version = HttpVersion.Version11,
            Content = content
        };

        // Set JSON Accept header
        request.Headers.Add("Accept", MediaTypeNames.Application.Json);
        AddAuthCookie(request);

        // Send the request
        var response = await WebClient.SendAsync(request);
        
        // Check if the response is successful
        if (!response.IsSuccessStatusCode)
        {
            // Handle error responses
            if (response.StatusCode == HttpStatusCode.BadRequest) // 400 response
            {
                // Read the response content
                string responseContent = await response.Content.ReadAsStringAsync();
                
                // Log or handle the error details
                Console.WriteLine($"Bad Request: {responseContent}");
                // You can also try to deserialize the error response into a model
                // var errorDetails = JsonConvert.DeserializeObject<ErrorModel>(responseContent);
                
                // Optionally throw a custom exception with error details
                throw new HttpRequestException($"Bad Request (400): {responseContent}", null, response.StatusCode);
            }
            
            // For other error status codes
            response.EnsureSuccessStatusCode(); // This will throw for other error codes
        }

        // Return true to indicate success
        return true;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            // Ensure proper cleanup without throwing exceptions
            try 
            {
                Cleanup().GetAwaiter().GetResult();
            }
            catch
            {
                // Ignore cleanup errors during disposal
            }

            WebClient.Dispose();

            _disposed = true;
        }

        base.Dispose(disposing);
    }
}