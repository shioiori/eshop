using Ordering.Domain.Abstractions;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Modals
{
    public record OrderItemDto
        (Guid OrderId, 
        Guid ProductId, 
        int Quantity, 
        decimal Price);
}
