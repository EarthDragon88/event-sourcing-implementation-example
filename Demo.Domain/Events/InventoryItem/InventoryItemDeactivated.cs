using EventSourcing.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.Events.InventoryItem
{
    public class InventoryItemDeactivated : Event
    {
        public InventoryItemDeactivated(Guid id)
            : base(id)
        {
        }
    }
}
