using Ordering.Domain.Abstractions;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Modals
{
    public class Customer : Entity<CustomerId>
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        public static Customer Create(string name, string email)
        {
            return Create(CustomerId.Of(Guid.NewGuid()), name, email);
        }

        public static Customer Create(CustomerId id, string name, string email)
        {
            var customer = new Customer
            {
                Id = id,
                Name = name,
                Email = email
            };
            return customer;
        }
    }
}
