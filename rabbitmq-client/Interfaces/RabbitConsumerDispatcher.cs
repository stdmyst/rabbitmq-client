using static System.GC;

namespace rabbitmq_client.Interfaces;

public interface IRabbitConsumerDispatcher : IDisposable
{ 
    Task ConfigureAndRunConsumers();
}

public abstract class RabbitConsumerDispatcher(IRabbitClientFactory rabbitClientFactory) : IRabbitConsumerDispatcher
{
    private bool _disposed;
    
    protected IRabbitClientFactory RabbitClientFactory { get; } = rabbitClientFactory;
    protected readonly List<IRabbitConsumer> Consumers = new();

    public abstract Task ConfigureAndRunConsumers();

    public void Dispose()
    {
        Dispose(true);
        SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        _disposed = true;
        if (disposing)
            foreach (var consumer in Consumers)
                consumer.Dispose();
    }
}