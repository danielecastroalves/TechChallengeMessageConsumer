using System.Text;
using Bogus;
using FintechMessageConsumer.Infrastructure.RabbitMQ;
using Moq;
using RabbitMQ.Client;

namespace FintechMessageConsumer.Tests.UnitTests.Infrastructure.RabbitMQ;

public sealed class MessagePublisherServiceTest
{
    private readonly Mock<IConnection> _mockConnection;
    private readonly Mock<IModel> _mockChannel;
    private readonly MessagePublisherService _service;
    private readonly Faker _faker;

    public MessagePublisherServiceTest()
    {
        _mockConnection = new Mock<IConnection>();
        _mockChannel = new Mock<IModel>();
        _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);
        _service = new MessagePublisherService(_mockConnection.Object);
        _faker = new Faker();
    }

    [Fact]
    public void PublishMessage_ShouldCreateChannel()
    {
        // Arrange
        var message = _faker.Lorem.Sentence();
        var queueName = _faker.Random.AlphaNumeric(10);

        // Act
        _service.PublishMessage(message, queueName);

        // Assert
        _mockConnection.Verify(c => c.CreateModel(), Times.Once);
    }

    [Fact]
    public void PublishMessage_ShouldDeclareQueue()
    {
        // Arrange
        var message = _faker.Lorem.Sentence();
        var queueName = _faker.Random.AlphaNumeric(10);

        // Act
        _service.PublishMessage(message, queueName);

        // Assert
        _mockChannel.Verify(c => c.QueueDeclare(
            queueName,
            false,
            false,
            false,
            null), Times.Once);
    }

    [Fact]
    public void PublishMessage_WithDurableTrue_ShouldDeclareQueueAsDurable()
    {
        // Arrange
        var message = _faker.Lorem.Sentence();
        var queueName = _faker.Random.AlphaNumeric(10);

        // Act
        _service.PublishMessage(message, queueName, durable: true);

        // Assert
        _mockChannel.Verify(c => c.QueueDeclare(
            queueName,
            true,
            false,
            false,
            null), Times.Once);
    }

    [Fact]
    public void PublishMessage_ShouldDisposeChannel()
    {
        // Arrange
        var message = _faker.Lorem.Sentence();
        var queueName = _faker.Random.AlphaNumeric(10);

        // Act
        _service.PublishMessage(message, queueName);

        // Assert
        _mockChannel.Verify(c => c.Dispose(), Times.Once);
    }
}
