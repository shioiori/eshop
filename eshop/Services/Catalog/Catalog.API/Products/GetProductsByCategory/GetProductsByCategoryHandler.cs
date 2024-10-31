namespace Catalog.API.Products.GetProductByCategory
{
    public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;
    public record GetProductsByCategoryResult(IEnumerable<Product> Products);
    public class GetProductsByCategoryHandler(IDocumentSession session, ILogger<GetProductsByCategoryHandler> logger)
        : IRequestHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
    {
        public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductsQueryHandler.Handler called with {@Query}", query);
            var products = await session.Query<Product>()
                            .Where(p => p.Category.Contains(query.Category))
                            .ToListAsync();
            return new GetProductsByCategoryResult(products);
        }
    }
}
