using FintechMessageConsumer.Infrastructure.Mongo.Contexts.Interfaces;
using FintechMessageConsumer.Infrastructure.Mongo.Utils;
using FintechMessageConsumer.Infrastructure.Mongo.Utils.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FintechMessageConsumer.Infrastructure.Mongo.Contexts
{
    public class MongoContext : IMongoContext
    {
        private readonly IMongoConnection _connection;

        public int DefaultTtlDays { get; set; }

        public MongoContext
        (
            IMongoConnection mongoConnection,
            IOptions<MongoConnectionOptions> options
        )
        {
            _connection = mongoConnection;
            DefaultTtlDays = options.Value.DefaultTtlDays;
        }

        public IMongoDatabase GetDatabase() => _connection.GetDatabase();

        public IMongoCollection<T> GetCollection<T>(string? name = null)
        {
            var database = _connection.GetDatabase();

            IMongoCollection<T> collection = name != null
                ? database.GetCollection<T>(name)
                : GetCollection<T>(database);

            return collection;
        }

        public static IMongoCollection<T> GetCollection<T>(IMongoDatabase database)
        {
            var attrs = typeof(T).GetCustomAttributes(typeof(CollectionNameAttribute), false)
                                 .OfType<CollectionNameAttribute>().FirstOrDefault();

            var collectionName = attrs?.Name ?? typeof(T).Name;

            return database.GetCollection<T>(collectionName);
        }
    }
}
