using FintechMessageConsumer.Infrastructure.Mongo.Utils;

namespace FintechMessageConsumer.Tests.MockAssistant.Infrastructure.Mongo
{
    public static class MongoConnectionOptionsMock
    {
        public static MongoConnectionOptions Get
        (
            int defaultTtlDays = 1,
            string schema = "schema",
            string connectionString = "connectionString"
        ) => new()
        {
            DefaultTtlDays = defaultTtlDays,
            Schema = schema,
            ConnectionString = connectionString
        };
    }
}
