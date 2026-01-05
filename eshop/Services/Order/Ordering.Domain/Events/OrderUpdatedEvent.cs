using Ordering.Domain.Abstractions;
using Ordering.Domain.Modals;

namespace Ordering.Domain.Events
{
    public class OrderUpdatedEvent(Order order) : IDomainEvent;
}
