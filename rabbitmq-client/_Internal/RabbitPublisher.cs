using System.Text;
using rabbitmq_client.Interfaces;
using RabbitMQ.Client;

namespace rabbitmq_client._Internal;

internal sealed class RabbitPublisher(IChannel channel) : RabbitClient(channel), IRabbitPublisher
{
    public async Task BasicPublish(
        string routingKey, 
        string exchange, 
        string body, 
        bool mandatory = false, 
        IDictionary<string, object> headers = null,
        CancellationToken cancellationToken = default)
    {
        var asBytes = Encoding.UTF8.GetBytes(body);
        
        await BasicPublish(routingKey, exchange, asBytes, mandatory, headers, cancellationToken);
    }
    
    public async Task BasicPublish(
        string routingKey, 
        string exchange, 
        ReadOnlyMemory<byte> body,
        bool mandatory = false,
        IDictionary<string, object> headers = null,
        CancellationToken cancellationToken = default)
    {
        var basicProperties = new BasicProperties
        {
            Headers = headers ?? new Dictionary<string, object>()
        };
        
        await Channel.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, body, cancellationToken);
    }
}