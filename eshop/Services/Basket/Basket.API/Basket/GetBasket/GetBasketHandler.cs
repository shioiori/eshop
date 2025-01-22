using Basket.API.Data;

namespace Basket.API.Basket.CreateBasket
{
  public record GetBasketQuery(string userName) : IQuery<GetBasketResult>;
  public record GetBasketResult(ShoppingCart Cart);
  public class GetBasketQueryHandler(IBasketRepository repository) : IQueryHandler<GetBasketQuery, GetBasketResult>
  {
    public async Task<GetBasketResult> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
      var cart = await repository.GetBasket(request.userName, cancellationToken);
      return new GetBasketResult(cart);
    }
  }
}
