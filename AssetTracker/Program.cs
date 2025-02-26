using AssetTracker.Services;
using AssetTracker.Repositories;
using Alpaca.Markets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IWatchlistService, WatchlistService>();

builder.Services.AddSingleton<IWatchlistRepository, WatchlistRepository>();

builder.Services.AddHostedService<AlpacaStockMarketService>();  // Registering the hosted service directly

builder.Services.AddSingleton<IAlpacaStockMarketService, AlpacaStockMarketService>();


builder.Services.AddSingleton<IAlphaVantageStockMarketService, AlphaVantageStockMarketService>();

builder.Services.AddSingleton<IStockService, StockService>();

builder.Services.AddSingleton<IPositionService, PositionService>();

builder.Services.AddSingleton<IUserRepository, UserRepository>();

builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

builder.Services.AddSingleton<IPortfolioService, PortfolioService>();

builder.Services.AddSingleton<IPortfolioRepository, PortfolioRepository>();

builder.Configuration.AddUserSecrets<Program>();


// Register the WebSocket background service

var redisConnection = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});

builder.Services.AddHttpClient();
//builder.Services.AddHttpClient<IAlphaVantageStockMarketService, AlphaVantageStockMarketService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

