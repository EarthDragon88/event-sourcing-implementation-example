using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.Commands
{
    public class CheckInItemsToInventory
    {
        public Guid InventoryItemId;
        public readonly int Count;
        public readonly int OriginalVersion;

        public CheckInItemsToInventory(Guid inventoryItemId, int count, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            Count = count;
            OriginalVersion = originalVersion;
        }
    }
}
