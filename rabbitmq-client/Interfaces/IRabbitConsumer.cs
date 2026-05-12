using rabbitmq_client._Internal;
using rabbitmq_client.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbitmq_client.Interfaces;

public interface IRabbitConsumer : IRabbitClient
{
    Task Consume(
        AsyncEventHandler<BasicDeliverEventArgs> eventHandler, 
        RabbitConsumerSettings settings);

    Task<BasicGetResult?> Get(
        string queue,
        bool autoAck = true,
        CancellationToken cancellationToken = default);
}