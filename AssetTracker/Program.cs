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

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
//Fetch secrets from AWS Secrets Manager
builder.Services.AddSingleton<IAmazonSecretsManager>(sp =>
    new AmazonSecretsManagerClient(RegionEndpoint.USEast2)); // Change region if needed
builder.Services.AddSingleton<AwsSecretsManagerHelper>();

var serviceProvider = builder.Services.BuildServiceProvider();
var secretsHelper = serviceProvider.GetRequiredService<AwsSecretsManagerHelper>();

// Fetch secrets from AWS
var secrets = Task.Run(() => secretsHelper.GetSecretsAsync("AssetTrackerSecrets")).Result;

foreach (var secret in secrets)
{
    var configKey = secret.Key.Replace("__", ":");
    builder.Configuration[configKey] = secret.Value;
    //Console.WriteLine($"config key={configKey}");
    //Console.WriteLine($"secret val={secret.Value}");

}
// UseMockStore setting
var useMockStore = builder.Configuration.GetValue<bool>("UseMockStore");
builder.Services.AddSingleton(new AppSettings { UseMockStore = useMockStore });

// Dependency Injection for Repositories
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
}

// Register Hosted Services
builder.Services.AddHostedService<AlpacaStockMarketService>();

// Register Stock Market Services
builder.Services.AddSingleton<IAlpacaStockMarketService, AlpacaStockMarketService>();
builder.Services.AddSingleton<IAlphaVantageStockMarketService, AlphaVantageStockMarketService>();

// Register Application Services
builder.Services.AddScoped<IWatchlistService, WatchlistService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<ICashFlowLogService, CashFlowLogService>();

// MongoDB Configuration
var mongoDbConnectionString = builder.Configuration["MongoDB:ConnectionString"];
var mongoClient = new MongoClient(mongoDbConnectionString);
var mongoDatabase = mongoClient.GetDatabase("PortfolioDB");
builder.Services.AddSingleton(mongoDatabase);

//var certFilePath = builder.Configuration["Kestrel:Endpoints:Https:Certificate:Path"];
//var keyFilePath = builder.Configuration["Kestrel:Endpoints:Https:CertificateKey:Path"];
//var certPassword = builder.Configuration["Cert:Password"];

//var certFilePath = builder.Configuration["Kestrel:Endpoints:Https:Certificate:Path"];

//var certFilePath = "/https/aspnetapp.pfx";
//var certPassword = "some.long.password.fllkwefiwejf23049uwlekjf.sEFWEFGR98^&$";
var certFilePath = builder.Configuration["Cert:Path"];
var certPassword = builder.Configuration["Cert:Password"];

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
        options.Listen(IPAddress.Any, 80);   // HTTP port
        options.Listen(IPAddress.Any, 5001, listenOptions =>
        {
            listenOptions.UseHttps();        // HTTPS port with a certificate
        });
        options.Listen(IPAddress.Any, 443, listenOptions =>
        {
            listenOptions.UseHttps();        // HTTPS port with a certificate
        });
    });


    // Use certificate (e.g., add to services for HTTPS)
    builder.Services.AddSingleton(certificate);
}
catch (CryptographicException ex)
{
    Console.WriteLine($"CryptographicException: {ex.Message}");
    throw;
}



// Hangfire Configuration
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

// Redis Configuration
var redisConnection = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]},password={builder.Configuration["Redis:Password"]}";
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnection; });

// Alpaca API Clients
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


// HTTP Client
builder.Services.AddHttpClient();

// JSON Configuration
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Authentication & Authorization
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

// Swagger Configuration
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
builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 443;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
// Build Application
var app = builder.Build();

// Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHangfireDashboard("/hangfire");

var hangfireTaskScheduler = app.Services.GetRequiredService<HangfireTaskScheduler>();
hangfireTaskScheduler.Configure();



app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

//using AssetTracker.Services;
//using AssetTracker.Repositories;
//using AssetTracker.Services.Interfaces;
//using AssetTracker.Repositories.MockRepositories;
//using AssetTracker.Repositories.MongoDBRepositories;
//using Microsoft.AspNetCore.Authentication.JwtBearer;

//using Alpaca.Markets;
//using System.Text.Json.Serialization;
//using MongoDB.Bson.Serialization;
//using MongoDB.Driver;
//using MongoDB.Bson.Serialization.Attributes;
//using AssetTracker.Repositories.Interfaces;
//using AssetTracker.Models;
//using System.Text.Json;
//using MongoDB.Bson;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using Microsoft.OpenApi.Models;
//using Hangfire;
//using Hangfire.Mongo;
//using Hangfire.Mongo.Migration.Strategies;
//using Hangfire.Mongo.Migration.Strategies.Backup;

//using Amazon.SecretsManager;



//var builder = WebApplication.CreateBuilder(args);

//var useMockStore = builder.Configuration.GetValue<bool>("UseMockStore");
//builder.Services.AddSingleton(new AppSettings { UseMockStore = useMockStore });
//// Add services to the container.
//if (useMockStore)
//{
//    // For Mock Store (Singleton Services)
//    builder.Services.AddSingleton<IPortfolioRepository, PortfolioRepository>();
//    builder.Services.AddSingleton<IUserRepository, UserRepository>();
//    builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
//    builder.Services.AddSingleton<IHistoricalPortfolioValueRepository, HistoricalPortfolioValueRepository>();
//    builder.Services.AddSingleton<IWatchlistRepository, WatchlistRepository>();
//}
//else
//{
//    // For Mongo-based Store (Singleton Services)
//    builder.Services.AddSingleton<IHistoricalPortfolioValueRepository, MongoHistoricalPortfolioValueRepository>();
//    builder.Services.AddSingleton<IOrderRepository, MongoOrderRepository>();
//    builder.Services.AddSingleton<IPortfolioRepository, MongoPortfolioRepository>();
//    builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
//    builder.Services.AddSingleton<IWatchlistRepository, MongoWatchlistRepository>();
//}

//// Register Hosted Services
//builder.Services.AddHostedService<AlpacaStockMarketService>();

//// Register Stock Market Services (Singletons)
//builder.Services.AddSingleton<IAlpacaStockMarketService, AlpacaStockMarketService>();
//builder.Services.AddSingleton<IAlphaVantageStockMarketService, AlphaVantageStockMarketService>();

//// Register Application Services (Scoped)
//builder.Services.AddScoped<IWatchlistService, WatchlistService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IPositionService, PositionService>();
//builder.Services.AddScoped<IStockService, StockService>();

//// Register Password and Authentication Services (Singletons)
//builder.Services.AddSingleton<IPasswordService, PasswordService>();
//builder.Services.AddSingleton<IAuthService, AuthService>();

//// PortfolioService is dependent on IPositionService, so it should be Scoped as well
//builder.Services.AddScoped<IPortfolioService, PortfolioService>();



//builder.Configuration.AddUserSecrets<Program>();

////BsonDefaults. = GuidRepresentationMode.V3;
//var mongoDbConnectionString = builder.Configuration.GetValue<string>("MongoDB:ConnectionString");
//var mongoClient = new MongoClient(mongoDbConnectionString);
//var mongoDatabase = mongoClient.GetDatabase("PortfolioDB"); // Replace "PortfolioDB" with your database name
//builder.Services.AddSingleton(mongoDatabase);

//// MongoDB setup for HangfireDB
//var hangfireDatabaseName = "HangfireDB"; // Separate database for Hangfire
//builder.Services.AddHangfire(config =>
//{
//    config.UseMongoStorage(mongoClient, hangfireDatabaseName, new MongoStorageOptions
//    {
//        MigrationOptions = new MongoMigrationOptions
//        {
//            MigrationStrategy = new DropMongoMigrationStrategy(),
//            BackupStrategy = new NoneMongoBackupStrategy()
//        },
//        Prefix = "hangfire" // Optional: Prefix for Hangfire collections
//    });
//});



//// Register the WebSocket background service
//builder.Services.AddTransient<HangfireTaskScheduler>();

//var redisConnection = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}";
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = redisConnection;
//});
//builder.Services.AddSingleton<IAlpacaTradingClient>(sp =>
//{
//    var config = sp.GetRequiredService<IConfiguration>();
//    var securityKey = new SecretKey(config["Alpaca:ApiKey"], config["Alpaca:ApiSecret"]);

//    return Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(securityKey);
//});

//builder.Services.AddSingleton<IAlpacaDataStreamingClient>(sp =>
//{
//    var config = sp.GetRequiredService<IConfiguration>();
//    var securityKey = new SecretKey(config["Alpaca:ApiKey"], config["Alpaca:ApiSecret"]);
//    return Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(securityKey);
//});

//builder.Services.AddHttpClient();
////builder.Services.AddHttpClient<IAlphaVantageStockMarketService, AlphaVantageStockMarketService>();

////builder.Services.AddControllers();
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {

//        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
//    });
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//        .AddJwtBearer(options =>
//        {
//            options.TokenValidationParameters = new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidIssuer = builder.Configuration["Jwt:Issuer"],
//                ValidAudience = builder.Configuration["Jwt:Audience"],
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
//            };
//        });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("RequireAuthenticatedUser", policy =>
//        policy.RequireAuthenticatedUser());  // Add custom policies if needed
//});
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT"
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });
//});

//var configuration = new ConfigurationBuilder()
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .AddEnvironmentVariables()
//    .Build();
//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseHangfireDashboard("/hangfire");

//var hangfireTaskScheduler = app.Services.GetRequiredService<HangfireTaskScheduler>();
//hangfireTaskScheduler.Configure(); // Configure recurring tasks like market updates


//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

