using BuildingBlocks.CQRS;
using Ordering.Domain.Modals;

namespace Ordering.Application.Orders.Queries.GetOrdersByCustomer
{
    public record GetOrdersByCustomerResult(IEnumerable<OrderDto> Orders);
    public record GetOrdersByCustomerQuery(Guid CustomerId) : IQuery<GetOrdersByCustomerResult>;
}
