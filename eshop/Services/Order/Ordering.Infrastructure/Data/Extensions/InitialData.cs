using Ordering.Domain.Enums;
using Ordering.Domain.Modals;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<Customer> Customers => new List<Customer>
        {
            Customer.Create(CustomerId.Of(new Guid("fa7498b1-49f4-4aca-b7b0-b85ca880e8d8")), "John Doe", "johndoe@yopmail.com"),
            Customer.Create(CustomerId.Of(new Guid("94cc30c1-e638-4d0c-b4e1-cff8eda7b9c9")), "Mike", "mike@yopmail.com")
        };
        public static IEnumerable<Product> Products => new List<Product>
        {
            Product.Create(ProductId.Of(new Guid("d6365f9d-7935-47e8-9d92-8b028fbecff9")), "Product 1", 10.0m),
            Product.Create(ProductId.Of(new Guid("19af86c5-587f-4505-924c-b621b77bc622")), "Product 2", 20.0m)
        };
        public static IEnumerable<Order> Orders
        {
            get
            {
                var order1 = Order.Create(OrderId.Of(new Guid("66e6b873-2497-407c-9d37-9909e54f866c")),
                    CustomerId.Of(new Guid("fa7498b1-49f4-4aca-b7b0-b85ca880e8d8")),
                    OrderName.Of("Order 1"),
                    Address.Of("123 Main St", "CityA", "johndoe@yopmail.com", "StateA", "12345", "CountryA", "100000"),
                    Address.Of("123 Main St", "CityA", "mike@yopmail.com", "StateA", "12345", "CountryA", "100000"),
                    Payment.Of("Credit", "**** **** **** 1234", "2030/12", "016", "Paypal"),
                    OrderStatus.Draft);
                order1.Add(ProductId.Of(new Guid("d6365f9d-7935-47e8-9d92-8b028fbecff9")), 2, 10.0m);
                order1.Add(ProductId.Of(new Guid("19af86c5-587f-4505-924c-b621b77bc622")), 3, 8.5m);
                return new List<Order> { order1 };
            }
        }
    }
}
