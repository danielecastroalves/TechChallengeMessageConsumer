using FintechMessageConsumer.Application.Common.Configurations;
using FintechMessageConsumer.WebApi.Consumer;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;

namespace FintechMessageConsumer.Tests.UnitTests.WebApi.Consumers
{
    public class ClientProfileConsumerTest
    {
        [Fact]
        public async Task ExecuteAsync_Should_ConsumeMessageAndInvokeMediator()
        {
            // Arrange
            var rabbitConnectionMock = new Mock<IConnection>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            IOptions<RabbitMqConfig> options = Options.Create(new RabbitMqConfig());
            var channelMock = new Mock<IModel>();

            var consumer = new ClientProfileConsumer(
                rabbitConnectionMock.Object,
                serviceProviderMock.Object,
                options
            );

            // Setup mocks
            rabbitConnectionMock.Setup(x => x.CreateModel()).Returns(channelMock.Object);

            // Act
            await consumer.StartAsync(CancellationToken.None);

            // Assert
            channelMock
                .Verify(x => x.BasicConsume(null, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), null, It.IsAny<IBasicConsumer>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_CreateConsumerAndConsumeMessage()
        {
            // Arrange
            var rabbitConnectionMock = new Mock<IConnection>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            IOptions<RabbitMqConfig> options = Options.Create(new RabbitMqConfig());
            var channelMock = new Mock<IModel>();

            var consumer = new ClientProfileConsumer(rabbitConnectionMock.Object, serviceProviderMock.Object, options);

            // Setup mocks
            rabbitConnectionMock.Setup(x => x.CreateModel()).Returns(channelMock.Object);

            // Act
            await consumer.StartAsync(CancellationToken.None);

            // Assert
            channelMock.Verify(x => x.BasicConsume(null, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), null, It.IsAny<IBasicConsumer>()), Times.Once);

            // Clean up
            await consumer.StopAsync(CancellationToken.None);
        }
    }
}
