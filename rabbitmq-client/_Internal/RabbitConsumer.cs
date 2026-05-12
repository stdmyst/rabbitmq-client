using rabbitmq_client.Interfaces;
using rabbitmq_client.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbitmq_client._Internal;

internal class RabbitConsumer(IChannel channel) : RabbitClient(channel), IRabbitConsumer
{
    public async Task Consume(
        AsyncEventHandler<BasicDeliverEventArgs> eventHandler, 
        RabbitConsumerSettings settings)
    {
        if (settings.Qos is not null)
        {
            await ConfigureQos(settings.Qos);
        }
        
        var queue = await DeclareQueue(settings.QueueSettings);
        await Channel.QueueBindAsync(queue.QueueName, settings.Exchange, settings.RoutingKey);
        
        var consumer = new AsyncEventingBasicConsumer(Channel);
        
        consumer.ReceivedAsync += settings.AckSettings.AutoAck 
            ? eventHandler 
            : (sender, @event) => AckHandler(sender, @event, eventHandler, settings.AckSettings);

        await Channel.BasicConsumeAsync(queue.QueueName, settings.AckSettings.AutoAck, consumer);
    }
    
    public async Task<BasicGetResult?> Get(
        string queue, 
        bool autoAck = true, 
        CancellationToken cancellationToken = default)
    {
        var getResult = await Channel.BasicGetAsync(queue, autoAck, cancellationToken);

        return getResult;
    }
    
    private async Task ConfigureQos(QosSettings settings)
    {
        await Channel.BasicQosAsync(settings.PrefetchSize, settings.PrefetchCount, settings.Global);
    }
    
    private async Task<QueueDeclareOk> DeclareQueue(QueueSettings settings)
    {
        var queue = await Channel.QueueDeclareAsync(
            settings.QueueName,
            durable: settings.Durable,
            exclusive: settings.Exclusive,
            autoDelete: settings.AutoDelete,
            arguments: settings.Arguments,
            passive: settings.Passive,
            noWait: settings.NoWait);
        
        return queue;
    }

    private async Task AckHandler(
        object sender, 
        BasicDeliverEventArgs @event, 
        AsyncEventHandler<BasicDeliverEventArgs> baseEventHandler,
        AckSettings ackSettings)
    {
        try
        {
            await baseEventHandler(sender, @event);
            // Manually send consumer ack after event handled successfully.
            await Channel.BasicAckAsync(deliveryTag: @event.DeliveryTag, multiple: false);
        }
        catch
        {
            // Send nack if handler fails.
            await Channel.BasicNackAsync(
                deliveryTag: @event.DeliveryTag, 
                multiple: false, 
                requeue: ackSettings.RequeueOnFailure);
                    
            throw;
        }
    }
}