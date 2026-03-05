using Ordering.Domain.Enums;

namespace Ordering.Application.Dtos
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
