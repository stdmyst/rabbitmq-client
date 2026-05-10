using Microsoft.AspNetCore.Mvc;
using rabbitmq_client.Abstract;

namespace api_get_publish.Controllers;

[ApiController]
[Route("rabbit")]
public class RabbitController(IRabbitClientFactory rabbitClientFactory) : ControllerBase
{
    [HttpGet(Name = "get-message")]
    public async Task<IResult> Get(CancellationToken cancellationToken)
    {
        using var consumer = await rabbitClientFactory.CreateConsumer();
        var result = await consumer.Get("test-queue", autoAck: true, cancellationToken);
        
        return Results.Ok(result);
    }
    
    [HttpPost(Name = "publish-message")]
    public async Task<IResult> Post([FromBody] string message, CancellationToken cancellationToken)
    {
        using var publisher = await rabbitClientFactory.CreatePublisher();
        await publisher.BasicPublish(
            "test.rk",
            "test-exchange",
            message,
            cancellationToken: cancellationToken);

        return Results.Ok();
    }
}