namespace rabbitmq_client.Interfaces;

public interface IRabbitClientFactory
{
    Task<IRabbitPublisher> CreatePublisher();
    
    Task<IRabbitConsumer> CreateConsumer();
}