using BuildingBlocks.CQRS;
using Ordering.Application.Dtos;

namespace Ordering.Application.Orders.Queries.GetOrderByName
{
    public record GetOrderByNameResult(IEnumerable<OrderDto> Orders);
    public record GetOrderByNameQuery(string Name) : IQuery<GetOrderByNameResult>;
}
