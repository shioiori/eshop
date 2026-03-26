using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Catalog.API.Data;
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
    option.Connection(builder.Configuration.GetConnectionString("Database"));
    option.CreateDatabasesForTenants(c =>
    {
        c.ForTenant()
            .CheckAgainstPgDatabase()
            .WithOwner("postgres");
    });
}).UseLightweightSessions();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
    });
builder.Services.AddAuthorization();

if (builder.Environment.IsDevelopment())
{
  builder.Services.InitializeMartenWith<CatalogInitialData>();
}
builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("Database"));
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();

app.UseExceptionHandler();

app.UseHealthChecks("/health", new HealthCheckOptions
{
  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
