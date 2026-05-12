using Microsoft.Extensions.Logging;
using rabbitmq_client.Settings;
using RabbitMQ.Client;
using static System.GC;

namespace rabbitmq_client._Internal;

internal class RabbitConnectionManager(
    IConnectionFactory connectionFactory, 
    ILogger<RabbitConnectionManager> logger,
    RabbitConnectionSettings connectionSettings)
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
            
            AddConnectionEventHandlers();
        }
        catch (Exception e)
        {
            throw new Exception($"Could not connect to RabbitMQ. Error: {e.Message}", e);
        }
    }
    
    private void AddConnectionEventHandlers()
    {
        _connection?.ConnectionShutdownAsync += (_, @event) =>
        {
            logger.LogError("RabbitMQ connection shutdown event received. " +
                            "It will attempt to recover connection with {RecoveryIntervalSeconds} seconds interval. " +
                            "Error: {Error}.", 
                connectionSettings.NetworkRecoveryIntervalSeconds,
                @event.Exception);
            
            return Task.CompletedTask;
        };
        
        _connection?.ConnectionRecoveryErrorAsync += (_, @event) =>
        {
            logger.LogError("RabbitMQ connection recovery failed. Error: {Error}.", @event.Exception);
            
            return Task.CompletedTask;
        };
        
        _connection?.RecoverySucceededAsync += (_, _) =>
        {
            logger.LogInformation("RabbitMQ connection connection was successfully recovered.");
            
            return Task.CompletedTask;
        };
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