namespace Basket.API.Data
{
  public class BasketRepository(IDocumentSession session) : IBasketRepository
  {
    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
      session.Delete(userName);
      await session.SaveChangesAsync();
      return true;
    }

    public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
    {
      var basket = await session.LoadAsync<ShoppingCart>(userName, cancellationToken);
      return basket != null ? basket : throw new InvalidOperationException();
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
      session.Store(cart);
      await session.SaveChangesAsync(cancellationToken);
      return cart; 
    }
  }
}
