/// <summary>
/// Program entry point for the AssetTracker ASP.NET Core application.
/// Responsible for configuring services, middleware, authentication, repositories, Hangfire, Redis,
/// Alpaca clients, and SignalR.
/// </summary>

using AssetTracker.Services;
using AssetTracker.Repositories;
using AssetTracker.Services.Interfaces;
using AssetTracker.Repositories.MockRepositories;
using AssetTracker.Repositories.MongoDBRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Alpaca.Markets;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using AssetTracker.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;
using AssetTracker.Models;
using Amazon;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.AspNetCore.HttpsPolicy;
using System.Runtime.ConstrainedExecution;
using Hangfire.Dashboard.BasicAuthorization;
using AssetTracker.Helpers;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Load base and environment-specific configuration files and environment variables.
/// </summary>
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

/// <summary>
/// Register AWS Secrets Manager and helper for secure secret access.
/// </summary>
builder.Services.AddSingleton<IAmazonSecretsManager>(sp =>
    new AmazonSecretsManagerClient(RegionEndpoint.USEast2));
builder.Services.AddSingleton<AwsSecretsManagerHelper>();

var serviceProvider = builder.Services.BuildServiceProvider();
var secretsHelper = serviceProvider.GetRequiredService<AwsSecretsManagerHelper>();

/// <summary>
/// Fetch secrets from AWS Secrets Manager and inject into configuration.
/// </summary>
var secrets = Task.Run(() => secretsHelper.GetSecretsAsync("AssetTrackerSecrets")).Result;

foreach (var secret in secrets)
{
    var configKey = secret.Key.Replace("__", ":");
    builder.Configuration[configKey] = secret.Value;
}

/// <summary>
/// Determine which repository implementations to use based on configuration.
/// </summary>
var useMockStore = builder.Configuration.GetValue<bool>("UseMockStore");
builder.Services.AddSingleton(new AppSettings { UseMockStore = useMockStore });

/// <summary>
/// Register repository services depending on whether mocking is enabled.
/// </summary>
if (useMockStore)
{
    builder.Services.AddSingleton<ICashFlowLogRepository, CashFlowLogRepository>();
    builder.Services.AddSingleton<IPortfolioRepository, PortfolioRepository>();
    builder.Services.AddSingleton<IUserRepository, UserRepository>();
    builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
    builder.Services.AddSingleton<IHistoricalPortfolioValueRepository, HistoricalPortfolioValueRepository>();
    builder.Services.AddSingleton<IWatchlistRepository, WatchlistRepository>();
}
else
{
    builder.Services.AddSingleton<ICashFlowLogRepository, MongoCashFlowLogRepository>();
    builder.Services.AddSingleton<IHistoricalPortfolioValueRepository, MongoHistoricalPortfolioValueRepository>();
    builder.Services.AddSingleton<IOrderRepository, MongoOrderRepository>();
    builder.Services.AddSingleton<IPortfolioRepository, MongoPortfolioRepository>();
    builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
    builder.Services.AddSingleton<IWatchlistRepository, MongoWatchlistRepository>();
    builder.Services.AddSingleton<IUserSessionRepository, MongoUserSessionRepository>();
}

/// <summary>
/// Register services for stock market data providers.
/// </summary>
builder.Services.AddSingleton<IAlpacaStockMarketService, AlpacaStockMarketService>();
builder.Services.AddSingleton<IAlphaVantageStockMarketService, AlphaVantageStockMarketService>();

/// <summary>
/// Register application services for DI.
/// </summary>
builder.Services.AddScoped<IWatchlistService, WatchlistService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<ICashFlowLogService, CashFlowLogService>();
builder.Services.AddSingleton<IUserSessionManager, UserSessionManager>();

builder.Services.AddSingleton<SymbolSubscriptionManager>();

/// <summary>
/// Register Alpaca WebSocket service and hosted service.
/// </summary>
builder.Services.AddSingleton<AlpacaWebSocketService>();
builder.Services.AddSingleton<IAlpacaWebSocketService>(sp =>
    sp.GetRequiredService<AlpacaWebSocketService>());
builder.Services.AddHostedService(sp =>
    sp.GetRequiredService<AlpacaWebSocketService>());

/// <summary>
/// Add SignalR with JSON configuration for enum serialization.
/// </summary>
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = false;
    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

/// <summary>
/// Set up MongoDB client and inject PortfolioDB.
/// </summary>
var mongoDbConnectionString = builder.Configuration["MongoDB:ConnectionString"];
var mongoClient = new MongoClient(mongoDbConnectionString);
var mongoDatabase = mongoClient.GetDatabase("PortfolioDB");
builder.Services.AddSingleton(mongoDatabase);

/// <summary>
/// Load SSL certificate and configure Kestrel for HTTPS.
/// </summary>
var certFilePath = builder.Configuration["Cert:Path"];
var certPassword = builder.Configuration["Cert:Password"];

//var certFilePath = "certs/mycert.cer";
//var certPassword = "StrongCertPassword123";

//Console.WriteLine($"Certificate Path: {certFilePath}");
//Console.WriteLine($"Certificate Password: {certPassword}");

if (string.IsNullOrEmpty(certFilePath) || string.IsNullOrEmpty(certPassword))
{
    throw new Exception("Certificate file path or password not configured.");
}

try
{
    var certificate = new X509Certificate2(certFilePath, certPassword, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
    builder.WebHost.ConfigureKestrel((context, options) =>
    {
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.ServerCertificate = certificate;
        });
        options.Listen(IPAddress.Any, 80);
        options.Listen(IPAddress.Any, 5001, listenOptions =>
        {
            listenOptions.UseHttps();
        });
        options.Listen(IPAddress.Any, 443, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    });

    builder.Services.AddSingleton(certificate);
}
catch (CryptographicException ex)
{
    Console.WriteLine($"CryptographicException: {ex.Message}");
    throw;
}

/// <summary>
/// Configure Hangfire with MongoDB storage and register server.
/// </summary>
var hangfireDatabaseName = "HangfireDB";
builder.Services.AddHangfire(config =>
{
    config.UseMongoStorage(mongoClient, hangfireDatabaseName, new MongoStorageOptions
    {
        MigrationOptions = new MongoMigrationOptions
        {
            MigrationStrategy = new DropMongoMigrationStrategy(),
            BackupStrategy = new NoneMongoBackupStrategy()
        },
        Prefix = "hangfire"
    });
});
builder.Services.AddHangfireServer();
builder.Services.AddTransient<HangfireTaskScheduler>();

/// <summary>
/// Configure Redis and caching services.
/// </summary>
var redisConnection = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]},password={builder.Configuration["Redis:Password"]}";
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnection; });
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConnection);
});

/// <summary>
/// Set up Alpaca trading and streaming API clients.
/// </summary>
builder.Services.AddSingleton<IAlpacaTradingClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var securityKey = new SecretKey(config["Alpaca:ApiKey"], config["Alpaca:ApiSecret"]);
    return Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(securityKey);
});

builder.Services.AddSingleton<IAlpacaDataStreamingClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var securityKey = new SecretKey(config["Alpaca:ApiKey"], config["Alpaca:ApiSecret"]);
    return Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(securityKey);
});

/// <summary>
/// Configure HTTP and stock API clients.
/// </summary>
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IFinnhubStockMarketService, FinnhubService>();

/// <summary>
/// Configure JSON serialization.
/// </summary>
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
});

/// <summary>
/// Configure JWT authentication.
/// </summary>
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

/// <summary>
/// Configure Swagger with JWT support.
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

/// <summary>
/// Force HTTPS redirection.
/// </summary>
builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 443;
});

/// <summary>
/// Set up cookie policy for SameSite and consent handling.
/// </summary>
builder.Services.AddCookiePolicy(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

/// <summary>
/// Configure CORS for specific front-end origin.
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .SetIsOriginAllowed(_ => true) 
//            .AllowCredentials();

//    });
//});

/// <summary>
/// Build and configure the web application.
/// </summary>
var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// <summary>
/// Configure Hangfire dashboard with basic authentication.
/// </summary>
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new BasicAuthAuthorizationFilter(
        new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = false,
            SslRedirect = false,
            LoginCaseSensitive = true,
            Users = new[]
            {
                new BasicAuthAuthorizationUser
                {
                    Login = "Admin",
                    PasswordClear = builder.Configuration["Hangfire:Password"]
                }
            }
        }) }
});

/// <summary>
/// Run startup Hangfire task and preload Alpaca symbols.
/// </summary>
var hangfireTaskScheduler = app.Services.GetRequiredService<HangfireTaskScheduler>();
hangfireTaskScheduler.Configure();

var symbolService = app.Services.GetRequiredService<IAlpacaStockMarketService>();
await symbolService.InitializeAsync();

app.UseCors();
app.UseCookiePolicy();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

/// <summary>
/// Map SignalR hub endpoint.
/// </summary>
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MarketDataHub>("/hubs/marketdata");
});

app.Run();