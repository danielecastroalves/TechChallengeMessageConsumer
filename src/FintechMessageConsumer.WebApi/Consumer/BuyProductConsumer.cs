using System.Text;
using FintechMessageConsumer.Application.Common.Configurations;
using FintechMessageConsumer.Application.Features.Products;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FintechMessageConsumer.WebApi.Consumer
{
    /// <summary>
    /// Background Service for Buy Product RabbitMQ Consumer
    /// </summary>
    public class BuyProductConsumer : BackgroundService
    {
        private readonly IConnection _rabbitConnection;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqConfig _rabbitMqConfig;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rabbitConnection">RabbitMQ Connection</param>
        /// <param name="serviceProvider">Service Provider</param>
        /// <param name="options">RabbitMQ Config Options</param>
        public BuyProductConsumer
        (
            IConnection rabbitConnection,
            IServiceProvider serviceProvider,
            IOptions<RabbitMqConfig> options
        )
        {
            _rabbitConnection = rabbitConnection;
            _serviceProvider = serviceProvider;
            _rabbitMqConfig = options.Value;
        }

        /// <summary>
        /// ExecuteAsync
        /// </summary>
        /// <param name="stoppingToken">Cancellation Token</param>
        /// <returns>Task</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = _rabbitConnection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var productsEvent = JsonConvert.DeserializeObject<ProductsEvent>(Encoding.UTF8.GetString(body));

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(productsEvent!);

                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(queue: _rabbitMqConfig.BuyProductQueue,
                                autoAck: false,
                                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);                      
        }
    }
}
