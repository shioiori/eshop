using Identity.API;
using Identity.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        await scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>()
            .Database.MigrateAsync();

        await SeedData.InitializeAsync(scope.ServiceProvider);
    }
}

app.UseIdentityServices();

app.Run();
