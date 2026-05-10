namespace rabbitmq_client.Abstract;

public interface IRabbitClientFactory
{
    Task<IRabbitPublisher> CreatePublisher();
    
    Task<IRabbitConsumer> CreateConsumer();
}