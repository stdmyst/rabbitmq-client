using rabbitmq_client.Models;

namespace api_with_hosted_consumers;

public class AppSettings
{
    public RabbitSettings RabbitSettings { get; set; }
}

public class RabbitSettings
{
    public RabbitConnectionSettings ConnectionSettings { get; set; }
    public required string Exchange { get; set; }
    public required string QueueName { get; set; }
}