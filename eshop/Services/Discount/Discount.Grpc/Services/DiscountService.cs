using Discount.Grpc.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService(DiscountContext dbContext)
        : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<ProductDiscountModel> GetProductDiscount(
            GetProductDiscountRequest request, ServerCallContext context)
        {
            var now = DateTime.UtcNow;
            var discount = await dbContext.ProductDiscounts
                .FirstOrDefaultAsync(d =>
                    d.ProductName == request.ProductName &&
                    d.StartDate <= now &&
                    (d.EndDate == null || d.EndDate >= now));

            if (discount == null)
                return new ProductDiscountModel { Id = 0, ProductName = request.ProductName, Amount = 0 };

            return new ProductDiscountModel
            {
                Id = discount.Id,
                ProductName = discount.ProductName,
                Description = discount.Description,
                Amount = (double)discount.Amount
            };
        }

        public override async Task<OrderCouponModel> GetOrderCoupon(
            GetOrderCouponRequest request, ServerCallContext context)
        {
            var now = DateTime.UtcNow;
            var coupon = await dbContext.OrderCoupons
                .FirstOrDefaultAsync(c => c.Code == request.Code);

            if (coupon == null ||
                coupon.UsedCount >= coupon.MaxUsage ||
                coupon.StartDate > now ||
                (coupon.EndDate != null && coupon.EndDate < now) ||
                (double)coupon.MinOrderValue > request.OrderTotal)
            {
                return new OrderCouponModel { Id = 0, Amount = 0 };
            }

            return new OrderCouponModel
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountType = (int)coupon.DiscountType,
                Amount = (double)coupon.Amount,
                MinOrderValue = (double)coupon.MinOrderValue
            };
        }

        public override async Task<RedeemOrderCouponResponse> RedeemOrderCoupon(
            RedeemOrderCouponRequest request, ServerCallContext context)
        {
            var coupon = await dbContext.OrderCoupons
                .FirstOrDefaultAsync(c => c.Code == request.Code);

            if (coupon == null)
                return new RedeemOrderCouponResponse { Success = false };

            coupon.UsedCount++;
            await dbContext.SaveChangesAsync();
            return new RedeemOrderCouponResponse { Success = true };
        }
    }
}
