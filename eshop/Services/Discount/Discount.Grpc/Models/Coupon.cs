namespace Discount.Grpc.Models
{
    public class ProductDiscount
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class OrderCoupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DiscountType DiscountType { get; set; }
        public decimal Amount { get; set; }
        public decimal MinOrderValue { get; set; }
        public int MaxUsage { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public enum DiscountType
    {
        Fixed = 0,
        Percent = 1
    }
}
