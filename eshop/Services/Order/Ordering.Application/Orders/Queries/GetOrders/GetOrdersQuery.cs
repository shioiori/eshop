using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Ordering.Domain.Modals;

namespace Ordering.Application.Orders.Queries.GetOrders
{
    public record GetOrdersResult(PaginationResult<OrderDto> Orders);
    public record GetOrdersQuery(PaginationRequest PaginationRequest) : IQuery<GetOrdersResult>;
}
