using api_with_hosted_consumers;
using Microsoft.Extensions.Options;
using rabbitmq_client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitSettings>(builder.Configuration.GetSection("rabbitSettings"));

// Rabbit hosted service dependencies.
#if false
builder.Services.AddRabbitClientFactory(serviceProvider 
        => serviceProvider.GetRequiredService<IOptions<RabbitSettings>>()
            .Value.ConnectionSettings)
    .AddRabbitConsumerBackgroundService<RabbitLogEventDispatcher>();
#endif
builder.Services.AddRabbitClientFactory(builder.Configuration, sectionName: "RabbitSettings:ConnectionSettings");
builder.Services.AddRabbitConsumerBackgroundService<RabbitLogEventDispatcher>();

var app = builder.Build();

app.Run();