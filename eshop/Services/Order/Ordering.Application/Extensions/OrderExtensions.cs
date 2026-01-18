using Ordering.Application.Dtos;
using Ordering.Domain.Modals;

namespace Ordering.Application.Extensions
{
    public static class OrderExtensions
    {
        public static IEnumerable<OrderDto> ToOrderDtoList(this IEnumerable<Order> orders)
        {
            return orders.Select(x => new OrderDto(
                Id: x.Id.Value,
                CustomerId: x.CustomerId.Value,
                OrderName: x.OrderName.Value,
                BillingAddress: new AddressDto(x.BillingAddress.FirstName, x.BillingAddress.LastName, x.BillingAddress.EmailAddress, x.BillingAddress.AddressLine, x.BillingAddress.Country, x.BillingAddress.State, x.BillingAddress.Zipcode),
                ShippingAddress: new AddressDto(x.ShippingAddress.FirstName, x.ShippingAddress.LastName, x.ShippingAddress.EmailAddress, x.ShippingAddress.AddressLine, x.ShippingAddress.Country, x.ShippingAddress.State, x.ShippingAddress.Zipcode),
                Payment: new PaymentDto(x.Payment.CardName, x.Payment.CardNumber, x.Payment.Expiration, x.Payment.CVV, x.Payment.PaymentMethod),
                Status: x.Status,
                OrderItems: x.OrderItems.Select(o => new OrderItemDto(o.OrderId.Value, o.ProductId.Value, o.Quantity, o.Price)).ToList()
            ));
        }
    }
}
