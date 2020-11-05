using EventSourcing.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.Events.InventoryItem
{
    public class InventoryItemCreated : Event
    {
        public string Name { get; private set; }

        public InventoryItemCreated(Guid id, string name)
            : base(id)
        {
            Name = name;
        }
    }
}
