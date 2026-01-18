using Ordering.Application.Dtos;
using Ordering.Domain.Enums;

namespace Ordering.Domain.Modals
{
    public record OrderDto 
        (Guid Id, 
        Guid CustomerId, 
        string OrderName, 
        AddressDto BillingAddress, 
        AddressDto ShippingAddress, 
        PaymentDto Payment, 
        OrderStatus Status,
        List<OrderItemDto> OrderItems);
}
