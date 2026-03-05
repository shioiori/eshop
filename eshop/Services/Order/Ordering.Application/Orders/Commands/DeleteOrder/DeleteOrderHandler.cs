using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.Exceptions;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
    {
        public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await dbContext.Orders.FindAsync([command.OrderId], cancellationToken);
            if (order == null) throw new OrderNotFoundException(command.OrderId);
            dbContext.Orders.Remove(order);
            var result = await dbContext.SaveChangesAsync(cancellationToken);
            return new DeleteOrderResult(result > 0);
        }
    }
}
