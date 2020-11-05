using Demo.Domain.Events.InventoryItem;
using Demo.WebClient.DbModels;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.WebClient.Handlers
{
    public class ItemsRemovedFromInventoryHandler : IHandleMessages<ItemsRemovedFromInventory>, IHandleMessages<IFailed<ItemsRemovedFromInventory>>
    {
        private readonly IBus _bus;
        private readonly InventoryContext _context;

        public ItemsRemovedFromInventoryHandler(IBus bus, InventoryContext context)
        {
            _bus = bus;
            _context = context;
        }

        public async Task Handle(ItemsRemovedFromInventory message)
        {
            // TODO Check receiver version is one higher than the previous version
            var item = _context
                .ItemDetail
                .Where(x => x.Id == message.AggregateId)
                .Single();

            item.Version = message.Version;
            item.CurrentCount -= message.Count;

            _context.SaveChanges();
        }

        public async Task Handle(IFailed<ItemsRemovedFromInventory> failed)
        {
            await _bus.Defer(TimeSpan.FromMinutes(1), failed.Message);
        }
    }
}
