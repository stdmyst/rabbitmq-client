using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using rabbitmq_client._Internal;
using rabbitmq_client.Interfaces;
using rabbitmq_client.Settings;
using RabbitMQ.Client;

namespace rabbitmq_client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitConsumerBackgroundService<TDispatcher>(this IServiceCollection services) 
        where TDispatcher : class, IRabbitConsumerDispatcher
    {
        services.AddSingleton<IRabbitConsumerDispatcher, TDispatcher>();
        services.AddHostedService<RabbitHostedService>();
        
        return services;
    }
    
    public static IServiceCollection AddRabbitClientFactory(
        this IServiceCollection services, 
        Func<IServiceProvider, RabbitConnectionSettings> getSettings)
    {
        services.AddSingleton<RabbitConnectionSettings>(implementationFactory: getSettings);
        services.AddSingleton<IConnectionFactory>(serviceProvider =>
        {
            var connectionSettings = serviceProvider.GetRequiredService<RabbitConnectionSettings>();
            
            return CreateFactory(connectionSettings);
        });
        
        return services.AddRabbitClientFactoryInternal();
    }
    
    public static IServiceCollection AddRabbitClientFactory(
        this IServiceCollection services, 
        IConfiguration configuration,
        string sectionName = "rabbitConnectionSettings")
    {
        var connectionSettings = configuration.GetSection(sectionName).Get<RabbitConnectionSettings>();
        if (connectionSettings is null)
            throw new Exception("Can not find RabbitMQ connection settings.");

        services.AddSingleton<RabbitConnectionSettings>(connectionSettings);
        services.AddSingleton<IConnectionFactory>(CreateFactory(connectionSettings));
        
        return services.AddRabbitClientFactoryInternal();
    }

    private static IServiceCollection AddRabbitClientFactoryInternal(this IServiceCollection services)
    {
        services.AddSingleton<RabbitConnectionManager>();
        services.AddSingleton<IRabbitClientFactory, RabbitClientFactory>();
        
        return services;
    }

    private static IConnectionFactory CreateFactory(RabbitConnectionSettings connectionSettings)
        => new ConnectionFactory
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
}