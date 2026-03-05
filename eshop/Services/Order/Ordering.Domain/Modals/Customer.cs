using Ordering.Domain.Abstractions;

namespace Ordering.Domain.Modals
{
    public class Customer : Entity<Guid>
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        public static Customer Create(string name, string email)
        {
            return Create(Guid.NewGuid(), name, email);
        }

        public static Customer Create(Guid id, string name, string email)
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
