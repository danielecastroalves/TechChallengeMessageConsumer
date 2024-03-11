using MongoDB.Driver;

namespace FintechMessageConsumer.Infrastructure.Mongo.Utils.Interfaces
{
    public interface IMongoConnection
    {
        IMongoDatabase GetDatabase();
    }
}
