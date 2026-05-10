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
        var rabbitSettings = serviceProvider.GetRequiredService<IOptions<RabbitSettings>>().Value;
        return new ConnectionFactory
        {
            HostName = rabbitSettings.Hostname,
            Port = rabbitSettings.Port,
            UserName = rabbitSettings.Username,
            Password = rabbitSettings.Password,
            VirtualHost = rabbitSettings.VirtualHost
        };
    })
    .AddRabbitClientFactory()
    .AddSingleton<IRabbitConsumerDispatcher, RabbitLogEventDispatcher>()
    .AddHostedService<RabbitHostedService>();

var app = builder.Build();

app.Run();