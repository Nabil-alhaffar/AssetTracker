using AssetTracker.Services;
using AssetTracker.Repositories;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//services.AddScoped<IStockService, StockService>();
builder.Services.AddSingleton<IStockService, StockService>();

builder.Services.AddSingleton<IPositionService, PositionService>();
//builder.Services.AddSingleton<IPositionRepository, PositionRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddSingleton<IPortfolioService, PortfolioService>();
builder.Services.AddSingleton<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

