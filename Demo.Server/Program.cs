using Demo.Domain.Aggregates;
using Demo.Domain.Commands;
using EventSourcing.Common.Domain;
using EventSourcing.SqlAdapter;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using System;

namespace Demo.Server
{
    class Program
    {
        private static string rabbitMqConnectionString = "amqp://localhost:5672";
        private static string sqlConnectionString = "Server=localhost;Database=t2;Trusted_Connection=True;";

        public static void Main(string[] args)
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                var commandHandlers = new CommandHandlers();

                activator.Handle<CreateInventoryItem>(commandHandlers.Handle);
                activator.Handle<CheckInItemsToInventory>(commandHandlers.Handle);
                activator.Handle<DeactivateInventoryItem>(commandHandlers.Handle);
                activator.Handle<RemoveItemsFromInventory>(commandHandlers.Handle);
                activator.Handle<RenameInventoryItem>(commandHandlers.Handle);

                var serverBus = Configure.With(activator)
                    .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "server"))
                    .Start();

                var eventStore = new SqlEventStore(sqlConnectionString, serverBus);
                var repository = new Repository<InventoryItem>(eventStore);

                commandHandlers.Repository = repository;
                serverBus.Subscribe<CreateInventoryItem>();
                serverBus.Subscribe<CheckInItemsToInventory>();
                serverBus.Subscribe<DeactivateInventoryItem>();
                serverBus.Subscribe<RemoveItemsFromInventory>();
                serverBus.Subscribe<RenameInventoryItem>();

                while (true)
                {
                    var key = Console.ReadKey();
                    if (key.KeyChar == 'q')
                        break;
                }
            }
        }
    }
}
