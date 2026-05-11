using System.Text;
using Microsoft.Extensions.Options;
using rabbitmq_client.Abstract;
using rabbitmq_client.Models;

namespace api_with_hosted_consumers;

public class RabbitLogEventDispatcher(
    IRabbitClientFactory rabbitClientFactory,
    IOptions<RabbitSettings> settings,
    ILogger<RabbitLogEventDispatcher> logger) : RabbitConsumerDispatcher(rabbitClientFactory)
{
    private static readonly string RoutingKey = "test.rk";
    
    private readonly RabbitSettings _rabbitSettings = settings.Value;
    
    public override async Task ConfigureAndRunConsumers()
    {
        var consumer = await RabbitClientFactory.CreateConsumer();
        
        // Add to consumers container.
        Consumers.Add(consumer);

        var consumerSettings = new RabbitConsumerSettings
        {
            Exchange = _rabbitSettings.Exchange,
            RoutingKey = RoutingKey,
            QueueSettings = new QueueSettings
            {
                QueueName = _rabbitSettings.QueueName,
                Arguments = new Dictionary<string, object?> { { "x-message-ttl", 864000000 } }
            }
        };
        
        await consumer.Consume(
            eventHandler: (_, @event) =>
            {
                var message = Encoding.UTF8.GetString(@event.Body.ToArray());
                logger.LogInformation($"Received message: {message}");
                
                return Task.CompletedTask;
            },
            settings: consumerSettings);
    }
}