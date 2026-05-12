using api_with_hosted_consumers;
using Microsoft.Extensions.Options;
using rabbitmq_client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<RabbitSettings>(builder.Configuration.GetSection("rabbitSettings"));;

// Rabbit service dependencies.
builder.Services.AddRabbitClientFactory(serviceProvider 
    => serviceProvider.GetRequiredService<IOptions<RabbitSettings>>()
        .Value.ConnectionSettings);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();