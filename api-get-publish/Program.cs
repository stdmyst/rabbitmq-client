using api_with_hosted_consumers;
using rabbitmq_client;
using rabbitmq_client.Abstract;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Rabbit service dependencies.
builder.Services.AddScoped<IConnectionFactory>(serviceProvider =>
    {
        var appSettings = serviceProvider.GetRequiredService<AppSettings>();
        var rabbitSettings = appSettings.RabbitSettings;
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

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();