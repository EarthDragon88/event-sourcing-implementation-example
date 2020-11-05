using Dapper;
using Demo.Domain.Events.InventoryItem;
using Demo.Infrastructure;
using EventSourcing.Common.Domain;
using EventSourcing.Common.Store;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace EventSourcing.SqlAdapter
{
    public class SqlEventStore : IEventStore
    {
        private string _connectionString;
        private IBus _bus;

        public SqlEventStore(string connectionString, IBus bus)
        {
            _connectionString = connectionString;
            _bus = bus;
        }

        public class SisoJsonDefaultContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(
                MemberInfo member,
                MemberSerialization memberSerialization)
            {
                //TODO: Maybe cache
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }

        public IEnumerable<Event> GetEventsForAggregate(Guid aggregateId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var events = connection.Query<string>(
                    @"SELECT Data
                      FROM EVENTS
                      WHERE AggregateId = '" + aggregateId.ToString() + @"'
                      ORDER BY Version"
                ).ToList();

                var previousEvents = events.Select(x => JsonConvert.DeserializeObject(x,
                    JsonInfrastructure.JsonSettings) as Event);


                if (events.Count() == 0)
                    throw new AggregateNotFoundException();

                return previousEvents;
            }
        }

        public void SaveEvents(Guid aggregateId, Type aggregateType, IEnumerable<Event> events, int expectedVersion)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    // This could be optimized to be in a single string of sql
                    // statements & expressions removing the need for round trips

                    var version = connection.QuerySingleOrDefault<int?>(
                        $"SELECT Version FROM Aggregates WHERE AggregateId = '{aggregateId}'", null, transaction);
                    if(version == null)
                    {
                        connection.Execute(
                            @"INSERT INTO Aggregates(AggregateId, Type, Version)
                                VALUES(@AggregateId, @Type, @Version)",
                                new {
                                    AggregateId = aggregateId,
                                    Type = aggregateType.Name,
                                    Version = 0 }, transaction);
                        version = 0;
                    }
                    else if (expectedVersion != version && expectedVersion != -1)
                        throw new ConcurrencyException();

                    var currVersion = expectedVersion;
                    foreach(var @event in events)
                    {
                        currVersion++;
                        @event.Version = currVersion;
                        var jsonData = JsonConvert.SerializeObject(
                            @event,
                            JsonInfrastructure.JsonSettings);
                        // cnn.Execute("update Table set val = @val where Id = @id", new {val, id = 1});

                        connection.Execute(
                            @"UPDATE Aggregates
                              SET Version = @Version
                              WHERE AggregateId = @Id",
                                new {Version = currVersion, Id = aggregateId},
                                transaction
                            );

                        // connection.Execute($"update aggregates set Version = {currVersion} WHERE AggregateId = $'{aggregateId}'");

                        connection.Execute(
                            @"INSERT INTO Events(EventId, AggregateId, TimeStamp, Version, Data)
                                VALUES(@EventId, @AggregateId, @TimeSTamp, @Version, @Data)",
                            new
                            {
                                EventId = Guid.NewGuid(),
                                AggregateId = aggregateId,
                                TimeStamp = DateTime.Now,
                                Version = currVersion,
                                Data = jsonData
                            }, transaction);

                        _bus.Publish(@event);
                    }


                    transaction.Commit();
                }

            }
        }
    }
}
