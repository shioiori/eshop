namespace Basket.API.Basket.CreateBasket
{
  public record GetBasketQuery(string userName) : IQuery<GetBasketResult>;
  public record GetBasketResult(ShoppingCart Cart);
  public class GetBasketQueryHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
  {
    public Task<GetBasketResult> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
