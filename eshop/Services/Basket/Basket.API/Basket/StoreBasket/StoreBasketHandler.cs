using Basket.API.Data;
using Discount.Grpc;
using MediatR;

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
  public class StoreBasketCommandHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto) 
        : ICommandHandler<StoreBasketCommand, StoreBasketResult>
  {
    public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
    {
      ShoppingCart cart = request.Cart;
      await DeductDiscount(cart, cancellationToken);
      await repository.StoreBasket(cart, cancellationToken);
      return new StoreBasketResult(cart.UserName);
    }

    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
      foreach (var item in cart.Items)
      {
        var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest() { ProductName = item.ProductName }, cancellationToken: cancellationToken);
        item.Price -= coupon.Amount;
      }
    }
  }
}
