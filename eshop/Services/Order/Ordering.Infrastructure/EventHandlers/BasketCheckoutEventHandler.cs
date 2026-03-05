using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Enums;
using Ordering.Domain.Modals;

namespace Ordering.Infrastructure.EventHandlers
{
    public class BasketCheckoutEventHandler(ISender sender) : IConsumer<BasketCheckoutEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var command = MapToCreateOrderCommand(context.Message);
            await sender.Send(command);
        }

        private static CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
        {
            var addressDto = new AddressDto(
                message.FirstName,
                message.LastName,
                message.EmailAddress,
                message.AddressLine,
                message.Country,
                message.State,
                message.ZipCode);

            var paymentDto = new PaymentDto(
                message.CardName,
                message.CardNumber,
                message.Expiration,
                message.CVV,
                message.PaymentMethod.ToString());

            var orderDto = new OrderDto(
                Id: Guid.NewGuid(),
                CustomerId: message.CustomerId,
                OrderName: message.UserName,
                BillingAddress: addressDto,
                ShippingAddress: addressDto,
                Payment: paymentDto,
                Status: OrderStatus.Pending,
                OrderItems: []);

            return new CreateOrderCommand(orderDto);
        }
    }
}
