namespace api_with_hosted_consumers;

public class AppSettings
{
    public RabbitSettings RabbitSettings { get; set; }
}

public class RabbitSettings
{
    public required string Hostname { get; set; }
    public int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string VirtualHost { get; set; } = "/";
    public required string Exchange { get; set; }
    public required string QueueName { get; set; }
}