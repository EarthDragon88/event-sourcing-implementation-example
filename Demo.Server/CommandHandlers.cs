using Demo.Domain.Aggregates;
using Demo.Domain.Commands;
using EventSourcing.Common.Domain;
using EventSourcing.SqlAdapter;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Server
{
    public class CommandHandlers
    {
        public IRepository<InventoryItem> Repository { get; set; }

        public async Task Handle(CreateInventoryItem message)
        {
            var item = new InventoryItem(message.InventoryItemId, message.Name);
            Repository.Save(item, -1);
        }

        public async Task Handle(DeactivateInventoryItem message)
        {
            var item = Repository.GetById(message.InventoryItemId);
            item.Deactivate();
            Repository.Save(item, message.OriginalVersion);
        }

        public async Task Handle(RemoveItemsFromInventory message)
        {
            var item = Repository.GetById(message.InventoryItemId);
            item.Remove(message.Count);
            Repository.Save(item, message.OriginalVersion);
        }

        public async Task Handle(CheckInItemsToInventory message)
        {
            var item = Repository.GetById(message.InventoryItemId);
            item.CheckIn(message.Count);
            Repository.Save(item, message.OriginalVersion);
        }

        public async Task Handle(RenameInventoryItem message)
        {
            var item = Repository.GetById(message.InventoryItemId);
            item.ChangeName(message.NewName);
            Repository.Save(item, message.OriginalVersion);
        }
    }
}
