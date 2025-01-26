using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data
{
    public class DiscountContext : DbContext
    {
        public DbSet<Coupon> Coupons { get; set; }
        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon()
                {
                    Id = 1,
                    ProductName = "Iphone X",
                    Description = "Sample 1",
                    Amount = 20
                },
                new Coupon()
                {
                    Id = 2,
                    ProductName = "Samsung Galaxy s24",
                    Description = "Sample 2",
                    Amount = 5
                }
            );
        }
    }
}
