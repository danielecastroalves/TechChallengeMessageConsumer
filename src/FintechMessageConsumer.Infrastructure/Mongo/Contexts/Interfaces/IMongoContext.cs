using MongoDB.Driver;

namespace FintechMessageConsumer.Infrastructure.Mongo.Contexts.Interfaces
{
    public interface IMongoContext
    {
        int DefaultTtlDays { get; }

        IMongoDatabase GetDatabase();

        IMongoCollection<T> GetCollection<T>(string? name = null);
    }
}
