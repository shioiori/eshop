using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.Extensions;

namespace Ordering.Application.Orders.Queries.GetOrdersByCustomer
{
    public class GetOrdersByCustomerHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrdersByCustomerQuery, GetOrdersByCustomerResult>
    {
        public Task<GetOrdersByCustomerResult> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
        {
            var orders = dbContext.Orders
                .Where(o => o.CustomerId.Value == request.CustomerId);
            return Task.FromResult(new GetOrdersByCustomerResult(orders.ToOrderDtoList()));
        }
    }
}
