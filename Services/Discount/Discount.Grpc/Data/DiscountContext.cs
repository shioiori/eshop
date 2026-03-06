using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data
{
    public class DiscountContext : DbContext
    {
        public DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public DbSet<OrderCoupon> OrderCoupons { get; set; }

        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDiscount>().HasData(
                new ProductDiscount
                {
                    Id = 1,
                    ProductName = "Iphone X",
                    Description = "iPhone X launch discount",
                    Amount = 20,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = null
                },
                new ProductDiscount
                {
                    Id = 2,
                    ProductName = "Samsung Galaxy s24",
                    Description = "Samsung promo",
                    Amount = 5,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = null
                }
            );

            modelBuilder.Entity<OrderCoupon>().HasData(
                new OrderCoupon
                {
                    Id = 1,
                    Code = "WELCOME10",
                    Description = "10% off for new customers",
                    DiscountType = DiscountType.Percent,
                    Amount = 10,
                    MinOrderValue = 0,
                    MaxUsage = 100,
                    UsedCount = 0,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = null
                }
            );
        }
    }
}
