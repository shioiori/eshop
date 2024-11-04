﻿namespace Basket.API.Basket.StoreBasket
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
  public class StoreBasketCommandHandler : ICommandHandler<StoreBasketCommand, StoreBasketResult>
  {
    public Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
