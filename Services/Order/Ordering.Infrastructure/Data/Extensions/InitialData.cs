using Ordering.Domain.Enums;
using Ordering.Domain.Modals;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<Customer> Customers => new List<Customer>
        {
            Customer.Create(new Guid("fa7498b1-49f4-4aca-b7b0-b85ca880e8d8"), "John Doe", "johndoe@yopmail.com"),
            Customer.Create(new Guid("94cc30c1-e638-4d0c-b4e1-cff8eda7b9c9"), "Mike", "mike@yopmail.com")
        };
        public static IEnumerable<Product> Products => new List<Product>
        {
            Product.Create(new Guid("d6365f9d-7935-47e8-9d92-8b028fbecff9"), "Product 1", 10.0m),
            Product.Create(new Guid("19af86c5-587f-4505-924c-b621b77bc622"), "Product 2", 20.0m)
        };
        public static IEnumerable<Order> Orders
        {
            get
            {
                var order1 = Order.Create(new Guid("66e6b873-2497-407c-9d37-9909e54f866c"),
                    new Guid("fa7498b1-49f4-4aca-b7b0-b85ca880e8d8"),
                    "Order 1",
                    Address.Of("John", "Doe", "johndoe@yopmail.com", "123 Main St", "CountryA", "StateA", "12345"),
                    Address.Of("John", "Doe", "johndoe@yopmail.com", "123 Main St", "CountryA", "StateA", "12345"),
                    Payment.Of("Credit", "**** **** **** 1234", "2030/12", "016", "Paypal"),
                    OrderStatus.Draft);
                order1.Add(new Guid("d6365f9d-7935-47e8-9d92-8b028fbecff9"), 2, 10.0m);
                order1.Add(new Guid("19af86c5-587f-4505-924c-b621b77bc622"), 3, 8.5m);
                return new List<Order> { order1 };
            }
        }
    }
}
