using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.Commands
{
    public class DeactivateInventoryItem
    {
        public readonly Guid InventoryItemId;
        public readonly int OriginalVersion;

        public DeactivateInventoryItem(Guid inventoryItemId, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            OriginalVersion = originalVersion;
        }
    }
}
