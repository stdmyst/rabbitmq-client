using rabbitmq_client.Interfaces;

namespace rabbitmq_client._Internal;

internal class RabbitClientFactory(RabbitConnectionManager connectionManager) : IRabbitClientFactory
{
    public async Task<IRabbitPublisher> CreatePublisher()
    {
        var channel = await connectionManager.GetChannel();
        var publisher = new RabbitPublisher(channel);
        
        return publisher;
    }
    
    public async Task<IRabbitConsumer> CreateConsumer()
    {
        var channel = await connectionManager.GetChannel();
        var consumer = new RabbitConsumer(channel);
        
        return consumer;
    }
}