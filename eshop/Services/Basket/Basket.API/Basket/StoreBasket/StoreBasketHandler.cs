using Basket.API.Data;

namespace Basket.API.Basket.StoreBasket
{
  public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
  public record StoreBasketResult(string UserName);
  public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
  {
    public StoreBasketCommandValidator()
    {
      RuleFor(x => x.Cart).NotEmpty();
      RuleFor(x => x.Cart.UserName).NotEmpty();
    }
  }
  public class StoreBasketCommandHandler(IBasketRepository repository) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
  {
    public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
    {
      ShoppingCart cart = request.Cart;
      await repository.StoreBasket(cart, cancellationToken);
      return new StoreBasketResult(cart.UserName);
    }
  }
}
