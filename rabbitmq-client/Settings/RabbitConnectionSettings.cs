namespace rabbitmq_client.Settings;

public class RabbitConnectionSettings
{
    public required string Hostname { get; set; }
    public int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string VirtualHost { get; set; } = "/";
}