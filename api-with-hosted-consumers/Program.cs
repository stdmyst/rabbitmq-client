using api_with_hosted_consumers;
using Microsoft.Extensions.Options;
using rabbitmq_client;
using rabbitmq_client.Abstract;
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
    .AddSingleton<IRabbitConsumerDispatcher, RabbitLogEventDispatcher>()
    .AddHostedService<RabbitHostedService>();

var app = builder.Build();

app.Run();