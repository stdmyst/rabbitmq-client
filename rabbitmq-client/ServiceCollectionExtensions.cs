using Microsoft.Extensions.DependencyInjection;
using rabbitmq_client._Internal;
using rabbitmq_client.Interfaces;
using rabbitmq_client.Settings;
using RabbitMQ.Client;

namespace rabbitmq_client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitClientFactory(
        this IServiceCollection services, 
        Func<IServiceProvider, RabbitConnectionSettings> getSettings)
    {
        services.AddSingleton(getSettings);
        services.AddSingleton<IConnectionFactory>(serviceProvider =>
        {
            var connectionSettings = serviceProvider.GetRequiredService<RabbitConnectionSettings>();
            
            return new ConnectionFactory
            {
                HostName = connectionSettings.Hostname,
                Port = connectionSettings.Port,
                UserName = connectionSettings.Username,
                Password = connectionSettings.Password,
                VirtualHost = connectionSettings.VirtualHost,
                AutomaticRecoveryEnabled = connectionSettings.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = connectionSettings.TopologyRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(connectionSettings.NetworkRecoveryIntervalSeconds)
            };
        });
        
        services.AddSingleton<RabbitConnectionManager>();
        services.AddSingleton<IRabbitClientFactory, RabbitClientFactory>();
        
        return services;
    }

    public static IServiceCollection AddRabbitConsumerBackgroundService<TDispatcher>(this IServiceCollection services) 
        where TDispatcher : class, IRabbitConsumerDispatcher
    {
        services.AddSingleton<IRabbitConsumerDispatcher, TDispatcher>();
        services.AddHostedService<RabbitHostedService>();
        
        return services;
    }
}