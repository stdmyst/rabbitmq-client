using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using rabbitmq_client.Interfaces;

namespace api_get_publish.Controllers;

[ApiController]
[Route("rabbit")]
public class RabbitController(IRabbitClientFactory rabbitClientFactory) : ControllerBase
{
    [HttpGet]
    [Route("get-message")]
    public async Task<IResult> Get(CancellationToken cancellationToken)
    {
        using var consumer = await rabbitClientFactory.CreateConsumer();
        var result = await consumer.Get("test-queue", autoAck: true, cancellationToken);
        
        return result is not null 
            ? Results.Ok(Encoding.UTF8.GetString(result.Body.ToArray())) 
            : Results.NotFound();
    }
    
    [HttpPost]
    [Route("publish-message")]
    public async Task<IResult> Post([FromBody] Message message, CancellationToken cancellationToken)
    {
        using var publisher = await rabbitClientFactory.CreatePublisher();
        await publisher.BasicPublish(
            "test.rk",
            "test-exchange",
            JsonSerializer.Serialize(message),
            cancellationToken: cancellationToken);

        return Results.Ok();
    }
    
    public record Message(string message);
}