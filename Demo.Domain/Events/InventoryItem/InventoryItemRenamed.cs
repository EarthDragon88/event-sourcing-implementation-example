using EventSourcing.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.Events.InventoryItem
{
    public class InventoryItemRenamed : Event
    {
        public string NewName { get; private set; }

        public InventoryItemRenamed(Guid id, string newName)
            : base(id)
        {
            NewName = newName;
        }
    }
}
