using Bogus;
using FintechMessageConsumer.Domain.Entities;
using FintechMessageConsumer.Infrastructure.Mongo.Contexts.Interfaces;
using FintechMessageConsumer.Infrastructure.Mongo.Repositories;
using FintechMessageConsumer.Tests.MotherObjects.Entities;
using MongoDB.Driver;
using Moq;
using Moq.AutoMock;

namespace FintechMessageConsumer.Tests.UnitTests.Infrastructure.Mongo.Repositories
{
    public class GenericRepositoryTest
    {
        private readonly AutoMocker _mocker;
        private readonly TestDataRepository _sut;
        public readonly Faker _faker;

        public GenericRepositoryTest()
        {
            _faker = new Faker();
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<TestDataRepository>();
        }

        [Fact]
        public async Task AddAsync_ShouldExecuteWithSuccess_WhenObjectIsValid()
        {
            // Arrange
            var entity = EntityMotherObject.ValidObject();

            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Setup(x => x.InsertOneAsync(entity,
                                             It.IsAny<InsertOneOptions>(),
                                             It.IsAny<CancellationToken>()))
                .Verifiable();

            _mocker
                .GetMock<IMongoContext>()
                .Setup(x => x.GetCollection<Entity>(null))
                .Returns(_mocker.GetMock<IMongoCollection<Entity>>().Object);

            // Act
            await _sut.AddAsync(entity, CancellationToken.None);

            // Assert
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Verify(x =>
                    x.InsertOneAsync(entity,
                                     It.IsAny<InsertOneOptions>(),
                                     It.IsAny<CancellationToken>()),
                    Times.Once);

            _mocker
                .GetMock<IMongoContext>()
                .Verify(x =>
                    x.GetCollection<Entity>(null),
                    Times.Once);
        }

        [Fact]
        public async Task GetByFilterAsync_ShouldExecuteWithSuccess_WhenFilterIsValid()
        {
            // Arrange         
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Entity>>(),
                                        It.IsAny<FindOptions<Entity>>(),
                                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mocker.GetMock<IAsyncCursor<Entity>>().Object);

            _mocker
                .GetMock<IMongoContext>()
                .Setup(x => x.GetCollection<Entity>(null))
                .Returns(_mocker.GetMock<IMongoCollection<Entity>>().Object);

            // Act
            await _sut.GetByFilterAsync(_ => true, CancellationToken.None);

            // Assert
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Verify(x =>
                    x.FindAsync(It.IsAny<FilterDefinition<Entity>>(),
                                It.IsAny<FindOptions<Entity>>(),
                                It.IsAny<CancellationToken>()),
                    Times.Once);

            _mocker
                .GetMock<IMongoContext>()
                .Verify(x => x.GetCollection<Entity>(null), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldExecuteWithSuccess_WhenIdIsValid()
        {
            // Arrange          
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Entity>>(),
                                        It.IsAny<FindOptions<Entity>>(),
                                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mocker.GetMock<IAsyncCursor<Entity>>().Object);

            _mocker
                .GetMock<IMongoContext>()
                .Setup(x => x.GetCollection<Entity>(null))
                .Returns(_mocker.GetMock<IMongoCollection<Entity>>().Object);

            // Act
            await _sut.GetByIdAsync(_faker.System.Random.Guid(), CancellationToken.None);

            // Assert
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Verify(x =>
                    x.FindAsync(It.IsAny<FilterDefinition<Entity>>(),
                                It.IsAny<FindOptions<Entity>>(),
                                It.IsAny<CancellationToken>()),
                    Times.Once);

            _mocker
                .GetMock<IMongoContext>()
                .Verify(x => x.GetCollection<Entity>(null), Times.Once);
        }

        [Fact]
        public async Task GetListByFilterAsync_ShouldExecuteWithSuccess_WhenFilterIsValid()
        {
            // Arrange          
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Entity>>(),
                                        It.IsAny<FindOptions<Entity>>(),
                                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mocker.GetMock<IAsyncCursor<Entity>>().Object);

            _mocker.GetMock<IMongoContext>()
                .Setup(x => x.GetCollection<Entity>(null))
                .Returns(_mocker.GetMock<IMongoCollection<Entity>>().Object);

            // Act
            _ = await _sut.GetListByFilterAsync(_ => true, CancellationToken.None);

            // Assert
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Verify(x =>
                    x.FindAsync(It.IsAny<FilterDefinition<Entity>>(),
                                It.IsAny<FindOptions<Entity>>(),
                                It.IsAny<CancellationToken>()),
                    Times.Once);

            _mocker.GetMock<IMongoContext>()
                .Verify(x =>
                    x.GetCollection<Entity>(null),
                    Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldExecuteWithSuccess_WhenObjectIsValid()
        {
            // Arrange
            var entity = EntityMotherObject.ValidObject();

            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Setup(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<Entity>>(),
                                              entity,
                                              It.IsAny<ReplaceOptions>(),
                                              It.IsAny<CancellationToken>()))
                .Verifiable();

            _mocker
                .GetMock<IMongoContext>()
                .Setup(x => x.GetCollection<Entity>(null))
                .Returns(_mocker.GetMock<IMongoCollection<Entity>>().Object);

            // Act
            await _sut.UpdateAsync(x => x.Id == entity.Id, entity, CancellationToken.None);

            // Assert
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Verify(x =>
                    x.ReplaceOneAsync(It.IsAny<FilterDefinition<Entity>>(),
                                      entity,
                                      It.IsAny<ReplaceOptions>(),
                                      It.IsAny<CancellationToken>()),
                    Times.Once);

            _mocker.GetMock<IMongoContext>()
                .Verify(x => x.GetCollection<Entity>(null), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldExecuteWithSuccess_WhenIdIsValid()
        {
            // Arrange
            var expectedDeleteResult = new DeleteResult.Acknowledged(1);

            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Entity>>(),
                                             It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDeleteResult);

            _mocker
                .GetMock<IMongoContext>()
                .Setup(x => x.GetCollection<Entity>(null))
                .Returns(_mocker.GetMock<IMongoCollection<Entity>>().Object);

            // Act
            await _sut.DeleteByIdAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert
            _mocker
                .GetMock<IMongoCollection<Entity>>()
                .Verify(x => x
                .DeleteOneAsync(
                    It.IsAny<FilterDefinition<Entity>>(),
                    It.IsAny<CancellationToken>()), Times.Once);

            _mocker
                .GetMock<IMongoContext>()
                .Verify(x => x.GetCollection<Entity>(null), Times.Once);
        }

        public class TestDataRepository(IMongoContext context)
            : GenericRepository<Entity>(context)
        { }
    }
}
