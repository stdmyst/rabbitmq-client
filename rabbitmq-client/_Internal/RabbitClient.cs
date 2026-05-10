using RabbitMQ.Client;
using static System.GC;

namespace rabbitmq_client._Internal;

public interface IRabbitClient : IDisposable;

internal abstract class RabbitClient(IChannel channel) : IRabbitClient
{
    private bool _disposed;
    
    protected readonly IChannel Channel = channel;
    
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
            Channel.Dispose();
    }
}