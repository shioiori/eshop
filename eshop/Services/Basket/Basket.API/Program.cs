using Basket.API.Data;
using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(config =>
{
  config.RegisterServicesFromAssembly(typeof(Program).Assembly);
  config.AddOpenBehavior(typeof(ValidationBehavior<,>));
  config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddCarter();
builder.Services.AddMarten(option =>
{
  option.Connection(builder.Configuration.GetConnectionString("Database")!);
  option.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
builder.Services.AddStackExchangeRedisCache(options =>
{
  options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// gRPC
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
  options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});

// cross-cutting concerns
builder.Services.AddHealthChecks()
  .AddNpgSql(builder.Configuration.GetConnectionString("Database"))
  .AddRedis(builder.Configuration.GetConnectionString("Redis"));
var app = builder.Build();
app.UseHealthChecks("/health", new HealthCheckOptions
{
  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
