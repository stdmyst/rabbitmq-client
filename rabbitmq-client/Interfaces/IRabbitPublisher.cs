using rabbitmq_client._Internal;

namespace rabbitmq_client.Interfaces;

public interface IRabbitPublisher : IRabbitClient
{
    Task BasicPublish(
        string routingKey, 
        string exchange, 
        string body,
        bool mandatory = false,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default);
    
    Task BasicPublish(
        string routingKey, 
        string exchange, 
        ReadOnlyMemory<byte> body,
        bool mandatory = false,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default);
}