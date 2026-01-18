using BuildingBlocks.CQRS;
using Ordering.Domain.Modals;

namespace Ordering.Application.Orders.Queries.GetOrderByName
{
    public record GetOrderByNameResult(IEnumerable<OrderDto> Orders);
    public record GetOrderByNameQuery(string Name) : IQuery<GetOrderByNameResult>;
}
