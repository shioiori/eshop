using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Ordering.Infrastructure.Data.Extensions
{
    public static class DatabaseExtension
    {
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var orderContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await orderContext.Database.MigrateAsync().ConfigureAwait(false);
            await SeedAsync(orderContext);
        }

        private static async Task SeedAsync(ApplicationDbContext dbContext)
        {
            await SeedCustomerAsync(dbContext);
            await SeedProductAsync(dbContext);
            await SeedOrderAsync(dbContext);
        }

        private static async Task SeedCustomerAsync(ApplicationDbContext dbContext)
        {
            if (!dbContext.Customers.Any())
            {
                await dbContext.Customers.AddRangeAsync(InitialData.Customers);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedProductAsync(ApplicationDbContext dbContext)
        {
            if (!dbContext.Products.Any())
            {
                await dbContext.Products.AddRangeAsync(InitialData.Products);
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedOrderAsync(ApplicationDbContext dbContext)
        {
            if (!dbContext.Orders.Any())
            {
                await dbContext.Orders.AddRangeAsync(InitialData.Orders);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
