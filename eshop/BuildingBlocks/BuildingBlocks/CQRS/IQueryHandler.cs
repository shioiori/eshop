using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.CQRS
{
    public interface IQueryHandler<TQuery> : IRequestHandler<TQuery, Unit>
        where TQuery : IQuery<Unit>
    { }
    public interface IQueryHandler<in TQuery, TRequest> : IRequestHandler<TQuery, TRequest>
        where TQuery : IQuery<TRequest>
        where TRequest : notnull
    {
    }
}
