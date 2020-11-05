using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.Commands
{
    public class CreateInventoryItem
    {
        public readonly Guid InventoryItemId;
        public readonly string Name;

        public CreateInventoryItem(Guid inventoryItemId, string name)
        {
            InventoryItemId = inventoryItemId;
            Name = name;
        }
    }
}
