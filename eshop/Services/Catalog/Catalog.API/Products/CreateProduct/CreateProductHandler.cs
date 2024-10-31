using Microsoft.Extensions.Logging;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(string Name, string Description, decimal Price, List<string> Category, string ImageFile) 
        : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid id);
    internal class CreateProductCommandHandler(IDocumentSession session, ILogger<CreateProductCommandHandler> logger)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("CreateProductCommandHandler.Handler called with {@Command}", command);
            var product = new Product()
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Category = command.Category,
                ImageFile = command.ImageFile,
            };
            
            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id);
        }
    }
}
