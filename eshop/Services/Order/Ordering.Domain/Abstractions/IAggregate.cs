using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Abstractions
{
    public interface IAggregate<T> : IAggregate, IEntity<T>
    {
    }
    public interface IAggregate
    {
        public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        IDomainEvent[] ClearDomainEvents();

    }
}
