using System.Text;
using FintechMessageConsumer.Application.Common.Services;
using RabbitMQ.Client;

namespace FintechMessageConsumer.Infrastructure.RabbitMQ
{
    public class MessagePublisherService : IMessagePublisherService
    {
        private readonly IConnection _connection;

        public MessagePublisherService(IConnection connection)
        {
            _connection = connection;
        }

        public void PublishMessage
        (
            string message,
            string queueName,
            bool durable = false
        )
        {
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                 durable: durable,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
