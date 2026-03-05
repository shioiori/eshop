using Ordering.Domain.Abstractions;

namespace Ordering.Domain.Modals
{
    public class OrderItem : Entity<Guid>
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        public static OrderItem Create(Guid orderId, Guid productId, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(quantity, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(price, 0);
            return new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                Price = price
            };
        }

    }
}
