using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourcing.Common.Domain
{
    public class Event
    {
        public Guid AggregateId { get; private set; }
        public int Version { get; set; }

        public Event(Guid eventId)
        {
            AggregateId = eventId;
        }
    }
}
