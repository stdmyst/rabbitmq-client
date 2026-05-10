using RabbitMQ.Client;
using static System.GC;

namespace rabbitmq_client._Internal;

internal class RabbitConnectionManager(IConnectionFactory connectionFactory)
{
    private bool _disposed;
    private IConnection? _connection;

    private bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public async Task<IChannel> GetChannel()
    {
        if (_connection is null)
        {
            await OpenConnection();
        }
        if (!IsConnected)
        {
            throw new Exception("RabbitMQ connection are not available.");
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return await _connection.CreateChannelAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
    
    private async Task OpenConnection()
    {
        if (IsConnected) return;
        
        try
        {
            _connection = await connectionFactory.CreateConnectionAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Could not connect to RabbitMQ. Error: {e.Message}", e);
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        _disposed = true;
        if (disposing)
            _connection?.Dispose();
    }
}