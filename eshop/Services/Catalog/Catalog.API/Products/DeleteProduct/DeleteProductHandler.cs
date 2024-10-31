
using Catalog.API.Products.GetProductById;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductCommand(Guid Id) : IQuery<DeleteProductResult>;
    public record DeleteProductResult(bool IsSuccess);
    public class DeleteProductHandler(IDocumentSession session, ILogger<DeleteProductHandler> logger) 
        : IQueryHandler<DeleteProductCommand, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("DeleteProductHandler.Handler called with {@Command}", command);
            
            session.Delete<Product>(command.Id);
            await session.SaveChangesAsync(cancellationToken);

            return new DeleteProductResult(true);
        }
    }
}
