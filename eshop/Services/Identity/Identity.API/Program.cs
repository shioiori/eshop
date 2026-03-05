using Identity.API;
using Identity.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.Services.CreateScope().ServiceProvider
        .GetRequiredService<ApplicationDbContext>()
        .Database.MigrateAsync();

    await SeedData.InitializeAsync(app.Services.CreateScope().ServiceProvider);
}

app.UseIdentityServices();

app.Run();
