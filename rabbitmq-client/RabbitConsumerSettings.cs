namespace rabbitmq_client;

public class RabbitConsumerSettings
{
    public required string Exchange { get; set; }
    public required string RoutingKey { get; set; }
    public AckSettings AckSettings { get; set; } = new();
    public required QueueSettings QueueSettings { get; set; }
}

public class AckSettings
{
    public bool AutoAck { get; set; } = true;
    public bool RequeueOnFailure { get; set; } = false;
}

public class QueueSettings
{
    public required string QueueName { get; set; }
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public IDictionary<string, object?>? Arguments { get; set; } = null;
    public bool Passive { get; set; } = false;
    public bool NoWait { get; set; } = false;
    public QosSettings? Qos { get; set; } = null;
}

public class QosSettings
{
    public uint PrefetchSize { get; set; } = 0;
    public ushort PrefetchCount { get; set; } = 1;
    public bool Global { get; set; } = false;
}