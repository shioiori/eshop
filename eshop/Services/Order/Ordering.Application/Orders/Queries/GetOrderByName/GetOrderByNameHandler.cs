using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.Extensions;

namespace Ordering.Application.Orders.Queries.GetOrderByName
{
    public class GetOrderByNameHandler(IApplicationDbContext dbContext) : IQueryHandler<GetOrderByNameQuery, GetOrderByNameResult>
    {
        public async Task<GetOrderByNameResult> Handle(GetOrderByNameQuery request, CancellationToken cancellationToken)
        {
            var orders = await dbContext.Orders
                .Where(o => o.OrderName.Value.ToLower().Contains(request.Name.ToLower()))
                .ToListAsync(cancellationToken);
            return new GetOrderByNameResult(orders.ToOrderDtoList());
                
        }
    }
}
