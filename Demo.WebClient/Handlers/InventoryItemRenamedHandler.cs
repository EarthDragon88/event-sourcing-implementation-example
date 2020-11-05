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
    public class InventoryItemRenamedHandler
        : IHandleMessages<InventoryItemRenamed>, IHandleMessages<IFailed<InventoryItemRenamed>>
    {
        private readonly IBus _bus;
        private readonly InventoryContext _context;

        public InventoryItemRenamedHandler(IBus bus, InventoryContext context)
        {
            _bus = bus;
            _context = context;
        }

        public async Task Handle(InventoryItemRenamed message)
        {
            // TODO Check receiver version is one higher than the previous version
            var detailItem = _context
                .ItemDetail
                .Where(x => x.Id == message.AggregateId)
                .Single();

            detailItem.Version = message.Version;
            detailItem.Name = message.NewName;

            var listItem = _context
                .ItemList
                .Where(x => x.Id == message.AggregateId)
                .Single();

            listItem.Name = message.NewName;

            _context.SaveChanges();
        }

        public async Task Handle(IFailed<InventoryItemRenamed> failed)
        {
            await _bus.Defer(TimeSpan.FromMinutes(1), failed.Message);
        }
    }
}
