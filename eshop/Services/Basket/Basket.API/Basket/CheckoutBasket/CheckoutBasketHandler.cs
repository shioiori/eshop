
using Basket.API.Data;
using Basket.API.Dtos;
using BuildingBlocks.Messaging.Events;
using Discount.Grpc;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto)
    : ICommand<CheckoutBasketResult>;
public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator
    : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckoutDto).NotNull().WithMessage("BasketCheckoutDto can't be null");
        RuleFor(x => x.BasketCheckoutDto.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class CheckoutBasketCommandHandler(
    IBasketRepository repository,
    IPublishEndpoint publishEndpoint,
    DiscountProtoService.DiscountProtoServiceClient discountProto)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await repository.GetBasket(command.BasketCheckoutDto.UserName, cancellationToken);
        if (basket == null)
            return new CheckoutBasketResult(false);

        var totalPrice = basket.TotalPrice;

        if (!string.IsNullOrWhiteSpace(command.BasketCheckoutDto.CouponCode))
        {
            var coupon = await discountProto.GetOrderCouponAsync(
                new GetOrderCouponRequest
                {
                    Code = command.BasketCheckoutDto.CouponCode,
                    OrderTotal = (double)totalPrice
                },
                cancellationToken: cancellationToken);

            if (coupon.Amount > 0)
            {
                totalPrice = coupon.DiscountType == (int)DiscountType.Percent
                    ? totalPrice * (1 - (decimal)coupon.Amount / 100)
                    : totalPrice - (decimal)coupon.Amount;

                totalPrice = Math.Max(0, totalPrice);

                await discountProto.RedeemOrderCouponAsync(
                    new RedeemOrderCouponRequest { Code = command.BasketCheckoutDto.CouponCode },
                    cancellationToken: cancellationToken);
            }
        }

        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.TotalPrice = totalPrice;

        await publishEndpoint.Publish(eventMessage, cancellationToken);
        await repository.DeleteBasket(command.BasketCheckoutDto.UserName, cancellationToken);

        return new CheckoutBasketResult(true);
    }
}

public enum DiscountType
{
    Fixed = 0,
    Percent = 1
}