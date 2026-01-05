using Ordering.Domain.Abstractions;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Modals
{
    public class OrderItem : Entity<OrderItemId>
    {
        public OrderId OrderId { get; private set; }
        public ProductId ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        public static OrderItem Create(OrderId orderId, ProductId productId, int quantity, decimal price)
        {
            ArgumentNullException.ThrowIfNull(orderId);
            ArgumentNullException.ThrowIfNull(productId);
            ArgumentOutOfRangeException.ThrowIfLessThan(quantity, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(price, 0);
            return new OrderItem
            {
                Id = OrderItemId.Of(Guid.NewGuid()),
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                Price = price
            };
        }

    }
}
