﻿using AssetTracker.Services;
using AssetTracker.Repositories;
using AssetTracker.Services.Interfaces;
using AssetTracker.Repositories.MockRepositories;
using AssetTracker.Repositories.MongoDBRepositories;

using Alpaca.Markets;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Models;
using System.Text.Json;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

var useMockStore = builder.Configuration.GetValue<bool>("UseMockStore");
builder.Services.AddSingleton(new AppSettings { UseMockStore = useMockStore });
// Add services to the container.
if (useMockStore)
{
    // For Mock Store (Singleton Services)
    builder.Services.AddSingleton<IPortfolioRepository, PortfolioRepository>();
    builder.Services.AddSingleton<IUserRepository, UserRepository>();
    builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
    builder.Services.AddSingleton<IHistoricalPortfolioValueRepository, HistoricalPortfolioValueRepository>();
    builder.Services.AddSingleton<IWatchlistRepository, WatchlistRepository>();
}
else
{
    // For Mongo-based Store (Singleton Services)
    builder.Services.AddSingleton<IHistoricalPortfolioValueRepository, MongoHistoricalPortfolioValueRepository>();
    builder.Services.AddSingleton<IOrderRepository, MongoOrderRepository>();
    builder.Services.AddSingleton<IPortfolioRepository, MongoPortfolioRepository>();
    builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
    builder.Services.AddSingleton<IWatchlistRepository, MongoWatchlistRepository>();
}

// Register Hosted Services
builder.Services.AddHostedService<AlpacaStockMarketService>();

// Register Stock Market Services (Singletons)
builder.Services.AddSingleton<IAlpacaStockMarketService, AlpacaStockMarketService>();
builder.Services.AddSingleton<IAlphaVantageStockMarketService, AlphaVantageStockMarketService>();

// Register Application Services (Scoped)
builder.Services.AddScoped<IWatchlistService, WatchlistService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IStockService, StockService>();

// Register Password and Authentication Services (Singletons)
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

// PortfolioService is dependent on IPositionService, so it should be Scoped as well
builder.Services.AddScoped<IPortfolioService, PortfolioService>();



builder.Configuration.AddUserSecrets<Program>();

//BsonDefaults. = GuidRepresentationMode.V3;
var mongoDbConnectionString = builder.Configuration.GetValue<string>("MongoDB:ConnectionString");
var mongoClient = new MongoClient(mongoDbConnectionString);
var mongoDatabase = mongoClient.GetDatabase("PortfolioDB"); // Replace "PortfolioDB" with your database name
builder.Services.AddSingleton(mongoDatabase);

// Register the WebSocket background service

var redisConnection = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});

builder.Services.AddHttpClient();
//builder.Services.AddHttpClient<IAlphaVantageStockMarketService, AlphaVantageStockMarketService>();

//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {

        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
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

