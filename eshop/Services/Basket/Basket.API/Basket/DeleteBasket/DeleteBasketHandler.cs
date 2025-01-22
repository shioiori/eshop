
using Basket.API.Data;

namespace Basket.API.Basket.DeleteBasket
{
  public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
  public record DeleteBasketResult(bool IsSuccess);
  public class DeleteCommandBasketHandler(IBasketRepository repository) : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
  {
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
    {
      var success = await repository.DeleteBasket(request.UserName, cancellationToken);
      return new DeleteBasketResult(success);
    }
  }
}
