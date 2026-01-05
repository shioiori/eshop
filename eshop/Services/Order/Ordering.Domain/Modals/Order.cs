using Ordering.Domain.Abstractions;
using Ordering.Domain.Enums;
using Ordering.Domain.Events;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Modals
{
    public class Order : Aggregate<OrderId>
    {
        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public CustomerId CustomerId { get; private set; }
        public OrderName OrderName { get; private set; }
        public Address BillingAddress { get; private set; }
        public Address ShippingAddress { get; private set; }
        public Payment Payment { get; private set; }
        public OrderStatus Status { get; private set; }

        public decimal TotalPrice
        {
            get => _orderItems.Sum(item => item.Price * item.Quantity);
            set { }
        }

        public static Order Create(OrderId id, CustomerId customerId, OrderName orderName, Address billingAddress, Address shippingAddress, Payment payment, OrderStatus status)
        {
            var order = new Order
            {
                Id = id,
                CustomerId = customerId,
                OrderName = orderName,
                BillingAddress = billingAddress,
                ShippingAddress = shippingAddress,
                Payment = payment,
                Status = status
            };

            order.AddDomainEvent(new OrderCreatedEvent(order));

            return order;
        }

        public void Update(OrderName orderName, Address billingAddress, Address shippingAddress, Payment payment, OrderStatus status)
        {
            OrderName = orderName;
            BillingAddress = billingAddress;
            ShippingAddress = shippingAddress;
            Payment = payment;
            Status = status;
            AddDomainEvent(new OrderUpdatedEvent(this));
        }
        
        public void Add(ProductId productId, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
            var orderItem = OrderItem.Create(Id, productId, quantity, price);
            _orderItems.Add(orderItem);
        }

        public void Remove(ProductId productId)
        {
            var orderItem = _orderItems.FirstOrDefault(item => item.ProductId == productId);
            if (orderItem != null)
            {
                _orderItems.Remove(orderItem);
            }
        }

    }
}
