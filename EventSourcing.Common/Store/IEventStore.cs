using EventSourcing.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourcing.Common.Store
{
    public interface IEventStore
    {
        void SaveEvents(Guid aggregateId, Type aggregateType, IEnumerable<Event> events, int expectedVersion);
        IEnumerable<Event> GetEventsForAggregate(Guid aggregateId);
    }
}
