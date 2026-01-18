using Ordering.Domain.Abstractions;
using Ordering.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Modals
{
    public class Product : Entity<ProductId>
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public static Product Create(string name, decimal price)
        {
            return Create(ProductId.Of(Guid.NewGuid()), name, price);
        }

        public static Product Create(ProductId id, string name, decimal price)
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
