using Ordering.Domain.Abstractions;
using Ordering.Domain.Modals;

namespace Ordering.Domain.Events
{
    public record OrderCreatedEvent(Order order) : IDomainEvent;
}
