using Microsoft.Extensions.DependencyInjection;
using rabbitmq_client._Internal;
using rabbitmq_client.Abstract;

namespace rabbitmq_client;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRabbitClientFactory(this IServiceCollection services)
    {
        services.AddSingleton<RabbitConnectionManager>();
        services.AddSingleton<IRabbitClientFactory, RabbitClientFactory>();
        
        return services;
    }
}