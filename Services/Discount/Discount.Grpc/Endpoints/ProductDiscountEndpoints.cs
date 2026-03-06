using Carter;
using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Endpoints;

public record CreateProductDiscountRequest(string ProductName, string Description, decimal Amount, DateTime StartDate, DateTime? EndDate);
public record UpdateProductDiscountRequest(string Description, decimal Amount, DateTime StartDate, DateTime? EndDate);

public class ProductDiscountEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/discounts/products").WithTags("Product Discounts");

        group.MapGet("/", GetAllAsync)
            .WithName("GetProductDiscounts")
            .Produces<List<ProductDiscount>>()
            .WithSummary("Get all product discounts");

        group.MapPost("/", CreateAsync)
            .WithName("CreateProductDiscount")
            .Produces<ProductDiscount>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create product discount");

        group.MapPut("/{id}", UpdateAsync)
            .WithName("UpdateProductDiscount")
            .Produces<ProductDiscount>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update product discount");

        group.MapDelete("/{id}", DeleteAsync)
            .WithName("DeleteProductDiscount")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete product discount");
    }

    private static async Task<IResult> GetAllAsync(DiscountContext db)
    {
        var discounts = await db.ProductDiscounts.ToListAsync();
        return Results.Ok(discounts);
    }

    private static async Task<IResult> CreateAsync(CreateProductDiscountRequest request, DiscountContext db)
    {
        var discount = new ProductDiscount
        {
            ProductName = request.ProductName,
            Description = request.Description,
            Amount = request.Amount,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        db.ProductDiscounts.Add(discount);
        await db.SaveChangesAsync();
        return Results.Created($"/discounts/products/{discount.Id}", discount);
    }

    private static async Task<IResult> UpdateAsync(int id, UpdateProductDiscountRequest request, DiscountContext db)
    {
        var discount = await db.ProductDiscounts.FindAsync(id);
        if (discount == null) return Results.NotFound();

        discount.Description = request.Description;
        discount.Amount = request.Amount;
        discount.StartDate = request.StartDate;
        discount.EndDate = request.EndDate;
        await db.SaveChangesAsync();
        return Results.Ok(discount);
    }

    private static async Task<IResult> DeleteAsync(int id, DiscountContext db)
    {
        var discount = await db.ProductDiscounts.FindAsync(id);
        if (discount == null) return Results.NotFound();

        db.ProductDiscounts.Remove(discount);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}
