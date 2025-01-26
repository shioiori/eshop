using Discount.Grpc.Data;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger)
        : DiscountProtoService.DiscountProtoServiceBase
    {
        public static CouponModel EmptyCoupon = new CouponModel() { Id = -1, Amount = 0 };
        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName.Equals(request.Coupon.ProductName));
            var model = coupon.Adapt<CouponModel>();
            return model;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
            if (coupon == null) return new DeleteDiscountResponse() { Success = false };
            dbContext.Coupons.Remove(coupon);
            await dbContext.SaveChangesAsync();
            return new DeleteDiscountResponse() { Success = true };
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName.Equals(request.ProductName));
            if (coupon == null)
            {
                return EmptyCoupon;
            }
            var model = coupon.Adapt<CouponModel>();
            return model;
        }

        public override Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            return base.UpdateDiscount(request, context);
        }
    }
}
