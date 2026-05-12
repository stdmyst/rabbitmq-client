using Microsoft.Extensions.DependencyInjection;
using rabbitmq_client._Internal;
using rabbitmq_client.Abstract;

namespace rabbitmq_client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitClientFactory(this IServiceCollection services)
    {
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