using api_with_hosted_consumers;
using Microsoft.Extensions.Options;
using rabbitmq_client;
using RabbitMQ.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<RabbitSettings>(builder.Configuration.GetSection("rabbitSettings"));

// Rabbit service dependencies.
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
    .AddRabbitClientFactory();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();