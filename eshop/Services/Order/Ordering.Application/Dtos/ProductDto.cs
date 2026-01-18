using Ordering.Domain.Abstractions;
using Ordering.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Modals
{
    public class ProductDTO : Entity<ProductId>
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public static ProductDTO Create(ProductId id, string name, decimal price)
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
