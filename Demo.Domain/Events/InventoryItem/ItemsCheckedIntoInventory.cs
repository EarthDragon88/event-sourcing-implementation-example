using EventSourcing.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.Events.InventoryItem
{
    public class ItemsCheckedIntoInventory : Event
    {
        public int Count { get; private set; }

        public ItemsCheckedIntoInventory(Guid id, int count)
            : base(id)
        {
            Count = count;
        }
    }
}
