using Ordering.Domain.Abstractions;

namespace Ordering.Domain.Modals
{
    public class ProductDTO : Entity<Guid>
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public static ProductDTO Create(Guid id, string name, decimal price)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfLessThan(price, 0);
            return new ProductDTO
            {
                Id = id,
                Name = name,
                Price = price
            };
        }
    }
}
