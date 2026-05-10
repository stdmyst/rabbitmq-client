using rabbitmq_client.Abstract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbitmq_client._Internal;

internal class RabbitConsumer(IChannel channel) : RabbitClient(channel), IRabbitConsumer
{
    public async Task Consume(
        AsyncEventHandler<BasicDeliverEventArgs> eventHandler, 
        RabbitConsumerSettings settings)
    {
        var queueSettings = settings.QueueSettings;
        
        await ConfigureQos(queueSettings.Qos);

        var queue = await Channel.QueueDeclareAsync(
            queueSettings.QueueName,
            durable: queueSettings.Durable,
            exclusive: queueSettings.Exclusive,
            autoDelete: queueSettings.AutoDelete,
            arguments: queueSettings.Arguments,
            passive: queueSettings.Passive,
            noWait: queueSettings.NoWait);
        
        await Channel.QueueBindAsync(queue.QueueName, settings.Exchange, settings.RoutingKey);
        
        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.ReceivedAsync += eventHandler;
        
        await Channel.BasicConsumeAsync(queueSettings.QueueName, settings.AutoAck, consumer);
    }
    
    public async Task<BasicGetResult> Get(
        string queue, 
        bool autoAck = true, 
        CancellationToken cancellationToken = default)
    {
        var getResult = await Channel.BasicGetAsync(queue, autoAck, cancellationToken);

        return getResult;
    }

    private async Task ConfigureQos(QosSettings? qosSettings)
    {
        if (qosSettings is not null)
            await Channel.BasicQosAsync(qosSettings.PrefetchSize, qosSettings.PrefetchCount, qosSettings.Global);
    }
}