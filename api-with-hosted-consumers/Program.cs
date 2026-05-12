using api_with_hosted_consumers;
using Microsoft.Extensions.Options;
using rabbitmq_client;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitSettings>(builder.Configuration.GetSection("rabbitSettings"));

// Rabbit hosted service dependencies.
builder.Services.AddSingleton<IConnectionFactory>(serviceProvider =>
    {
        var connectionSettings = serviceProvider.GetRequiredService<IOptions<RabbitSettings>>()
            .Value.ConnectionSettings;
        return new ConnectionFactory
        {
            HostName = connectionSettings.Hostname,
            Port = connectionSettings.Port,
            UserName = connectionSettings.Username,
            Password = connectionSettings.Password,
            VirtualHost = connectionSettings.VirtualHost
        };
    })
    .AddRabbitClientFactory()
    .AddRabbitConsumerBackgroundService<RabbitLogEventDispatcher>();

var app = builder.Build();

app.Run();