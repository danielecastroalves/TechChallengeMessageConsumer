using FintechMessageConsumer.Infrastructure.Mongo.Utils;
using Microsoft.Extensions.Options;
using Moq.AutoMock;

namespace FintechMessageConsumer.Tests.UnitTests.Infrastructure.Mongo.Utils
{
    public class MongoConnectionTest
    {
        private readonly AutoMocker _mocker;
        private readonly MongoConnectionOptions _connectionOptions;
        private MongoConnection? _mockedMongoConnection;

        public MongoConnectionTest()
        {
            _mocker = new AutoMocker();

            const string connectionString = "mongodb://root:root@localhost:27017";
            const string schema = "schema";
            const int defaultTtlDays = 10;

            _connectionOptions = new()
            {
                ConnectionString = connectionString,
                Schema = schema,
                DefaultTtlDays = defaultTtlDays
            };
        }

        [Fact]
        public void GetDatabase_ShouldThrowException_WhenConnectionStringIsEmpty()
        {
            // Arrange
            _connectionOptions.ConnectionString = string.Empty;

            _mocker
                .GetMock<IOptions<MongoConnectionOptions>>()
                .Setup(x => x.Value)
                .Returns(_connectionOptions);

            _mockedMongoConnection = _mocker.CreateInstance<MongoConnection>();

            // Act
            void act() => _mockedMongoConnection.GetDatabase();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Equal("Mongo ConnectionString is Null.", exception.Message);
        }

        [Fact]
        public void GetDatabase_ShouldThrowException_WhenDefaultDatabaseIsEmpty()
        {
            // Arrange
            _connectionOptions.Schema = string.Empty;

            _mocker
                .GetMock<IOptions<MongoConnectionOptions>>()
                .Setup(x => x.Value)
                .Returns(_connectionOptions);

            _mockedMongoConnection = _mocker.CreateInstance<MongoConnection>();

            // Act
            void act() => _mockedMongoConnection.GetDatabase();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Equal("Mongo Default Database is Null.", exception.Message);
        }

        [Fact]
        public void GetDatabase_ShouldReturnIMongoDatabase_WhenConnectionStringIsValid()
        {
            // Arrange
            _mocker
                .GetMock<IOptions<MongoConnectionOptions>>()
                .Setup(x => x.Value)
                .Returns(_connectionOptions);

            _mockedMongoConnection = _mocker.CreateInstance<MongoConnection>();

            // Act
            var response = _mockedMongoConnection.GetDatabase();

            // Assert
            Assert.NotNull(response);
        }
    }
}
