using Ordering.Domain.Abstractions;

namespace Ordering.Domain.Modals
{
    public class Product : Entity<Guid>
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public static Product Create(string name, decimal price)
        {
            return Create(Guid.NewGuid(), name, price);
        }

        public static Product Create(Guid id, string name, decimal price)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfLessThan(price, 0);
            return new Product
            {
                Id = id,
                Name = name,
                Price = price
            };
        }
    }
}
