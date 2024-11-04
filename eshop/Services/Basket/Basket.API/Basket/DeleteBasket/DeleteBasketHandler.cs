
namespace Basket.API.Basket.DeleteBasket
{
  public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
  public record DeleteBasketResult(bool IsSuccess);
  public class DeleteCommandBasketHandler : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
  {
    public Task<DeleteBasketResult> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
