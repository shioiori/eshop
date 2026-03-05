using Carter;
using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Endpoints;

public record CreateOrderCouponRequest(
    string Code, string Description, DiscountType DiscountType,
    decimal Amount, decimal MinOrderValue, int MaxUsage,
    DateTime StartDate, DateTime? EndDate);

public record UpdateOrderCouponRequest(
    string Description, DiscountType DiscountType,
    decimal Amount, decimal MinOrderValue, int MaxUsage,
    DateTime StartDate, DateTime? EndDate);

public class OrderCouponEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/discounts/orders").WithTags("Order Coupons");

        group.MapGet("/", GetAllAsync)
            .WithName("GetOrderCoupons")
            .Produces<List<OrderCoupon>>()
            .WithSummary("Get all order coupons");

        group.MapPost("/", CreateAsync)
            .WithName("CreateOrderCoupon")
            .Produces<OrderCoupon>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create order coupon");

        group.MapPut("/{id}", UpdateAsync)
            .WithName("UpdateOrderCoupon")
            .Produces<OrderCoupon>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update order coupon");

        group.MapDelete("/{id}", DeleteAsync)
            .WithName("DeleteOrderCoupon")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete order coupon");
    }

    private static async Task<IResult> GetAllAsync(DiscountContext db)
    {
        var coupons = await db.OrderCoupons.ToListAsync();
        return Results.Ok(coupons);
    }

    private static async Task<IResult> CreateAsync(CreateOrderCouponRequest request, DiscountContext db)
    {
        if (await db.OrderCoupons.AnyAsync(c => c.Code == request.Code))
            return Results.Problem("Coupon code already exists.", statusCode: StatusCodes.Status400BadRequest);

        var coupon = new OrderCoupon
        {
            Code = request.Code,
            Description = request.Description,
            DiscountType = request.DiscountType,
            Amount = request.Amount,
            MinOrderValue = request.MinOrderValue,
            MaxUsage = request.MaxUsage,
            UsedCount = 0,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        db.OrderCoupons.Add(coupon);
        await db.SaveChangesAsync();
        return Results.Created($"/discounts/orders/{coupon.Id}", coupon);
    }

    private static async Task<IResult> UpdateAsync(int id, UpdateOrderCouponRequest request, DiscountContext db)
    {
        var coupon = await db.OrderCoupons.FindAsync(id);
        if (coupon == null) return Results.NotFound();

        coupon.Description = request.Description;
        coupon.DiscountType = request.DiscountType;
        coupon.Amount = request.Amount;
        coupon.MinOrderValue = request.MinOrderValue;
        coupon.MaxUsage = request.MaxUsage;
        coupon.StartDate = request.StartDate;
        coupon.EndDate = request.EndDate;
        await db.SaveChangesAsync();
        return Results.Ok(coupon);
    }

    private static async Task<IResult> DeleteAsync(int id, DiscountContext db)
    {
        var coupon = await db.OrderCoupons.FindAsync(id);
        if (coupon == null) return Results.NotFound();

        db.OrderCoupons.Remove(coupon);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}
