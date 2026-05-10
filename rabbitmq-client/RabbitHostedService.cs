using Microsoft.Extensions.Hosting;
using rabbitmq_client.Abstract;

namespace rabbitmq_client;

public class RabbitHostedService(IRabbitConsumerDispatcher dispatcher) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await dispatcher.ConfigureAndRunConsumers();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        dispatcher.Dispose();
        
        return Task.CompletedTask;
    }
}