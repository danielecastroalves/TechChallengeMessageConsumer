namespace FintechMessageConsumer.Application.Common.Services
{
    public interface IMessagePublisherService
    {
        void PublishMessage(string message,
                            string queueName,
                            bool durable = false);
    }
}
