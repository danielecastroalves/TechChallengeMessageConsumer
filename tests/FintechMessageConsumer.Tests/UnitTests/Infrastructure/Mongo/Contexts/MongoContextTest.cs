using FintechMessageConsumer.Domain.Entities;
using FintechMessageConsumer.Infrastructure.Mongo.Contexts;
using FintechMessageConsumer.Infrastructure.Mongo.Utils;
using FintechMessageConsumer.Infrastructure.Mongo.Utils.Interfaces;
using FintechMessageConsumer.Tests.MockAssistant.Infrastructure.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Moq.AutoMock;

namespace FintechMessageConsumer.Tests.UnitTests.Infrastructure.Mongo.Contexts
{
    public class MongoContextTest
    {
        private readonly AutoMocker _mocker;
        private readonly MongoContext _mockedMongoContext;

        public MongoContextTest()
        {
            _mocker = new AutoMocker();

            var mongoConnection = _mocker.GetMock<IMongoConnection>();

            var mongoDatabase = _mocker.GetMock<IMongoDatabase>();

            var options = _mocker.GetMock<IOptions<MongoConnectionOptions>>();

            mongoConnection
                .Setup(mock => mock.GetDatabase())
                .Returns(mongoDatabase.Object);

            options
                .Setup(mock => mock.Value)
                .Returns(MongoConnectionOptionsMock.Get());

            _mockedMongoContext = _mocker.CreateInstance<MongoContext>();
        }

        [Fact]
        public void GetDatabase_ShouldReturnMongoDatabase_WhenRequested()
        {
            // Arrange
            var mongoDataBase = _mocker.GetMock<IMongoDatabase>();

            _mocker
                .GetMock<IMongoConnection>()
                .Setup(x => x.GetDatabase())
                .Returns(mongoDataBase.Object);

            // Act
            var result = _mockedMongoContext.GetDatabase();

            // Assert
            Assert.Equal(mongoDataBase.Object, result);
        }

        [Fact]
        public void GetDatabase_ShouldReturnCollection_WhenAskedForCollectionByName()
        {
            // Arrange
            var collection = new Mock<IMongoCollection<Entity>>();

            var mongoDataBase = _mocker.GetMock<IMongoDatabase>();

            mongoDataBase
            .Setup(x => x.GetCollection<Entity>(typeof(Entity).Name, null))
            .Returns(collection.Object);
            _mocker
                .GetMock<IMongoConnection>()
                .Setup(x => x.GetDatabase())
                .Returns(mongoDataBase.Object);

            // Act
            _mockedMongoContext.GetCollection<Entity>(typeof(Entity).Name);

            // Assert
            mongoDataBase.Verify(x =>
                x.GetCollection<Entity>(typeof(Entity).Name, null),
                Times.Exactly(1));
        }

        [Fact]
        public void GetDatabase_ShouldReturnCollection_WhenAskedForCollectionByType()
        {
            // Arrange
            var collection = new Mock<IMongoCollection<Entity>>();

            var mongoDataBase = _mocker.GetMock<IMongoDatabase>();

            mongoDataBase
                .Setup(x => x.GetCollection<Entity>(typeof(Entity).Name, null))
                .Returns(collection.Object);

            _mocker
                .GetMock<IMongoConnection>()
                .Setup(x => x.GetDatabase())
                .Returns(mongoDataBase.Object);

            // Act
            MongoContext.GetCollection<Entity>(mongoDataBase.Object);

            // Assert
            mongoDataBase.Verify(x =>
                x.GetCollection<Entity>(typeof(Entity).Name, null),
                Times.Exactly(1));
        }
    }
}
