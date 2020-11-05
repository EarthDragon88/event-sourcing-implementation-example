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
    public class InventoryItemCreatedHandler : IHandleMessages<InventoryItemCreated>, IHandleMessages<IFailed<InventoryItemCreated>>
    {
        private readonly IBus _bus;
        private readonly InventoryContext _context;

        public InventoryItemCreatedHandler(IBus bus, InventoryContext context)
        {
            _bus = bus;
            _context = context;
        }

        public async Task Handle(InventoryItemCreated message)
        {
            _context.Add(new InventoryItemDetails()
            {
                Id = message.AggregateId,
                Name = message.Name,
                Version = message.Version,
            });

            _context.Add(new InventoryItemList()
            {
                Id = message.AggregateId,
                Name = message.Name
            });

            _context.SaveChanges();
        }

        public async Task Handle(IFailed<InventoryItemCreated> failed)
        {
            await _bus.Defer(TimeSpan.FromMinutes(1), failed.Message);
        }
    }
}
