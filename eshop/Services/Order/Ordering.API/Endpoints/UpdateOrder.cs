using Carter;
using Mapster;
using MediatR;
using Ordering.Application.Orders.Commands.UpdateOrder;
using Ordering.Domain.Modals;

namespace Ordering.API.Endpoints
{
    public class UpdateOrder : ICarterModule
    {
        public record UpdateOrderRequest(OrderDto Order);
        public record UpdateOrderResponse(bool IsSuccess);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/orders/{id}", async (UpdateOrderRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateOrderCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateOrderResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateOrder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Order")
            .WithDescription("Update Order");
        }
    }
}
