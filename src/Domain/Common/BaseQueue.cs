using RabbitMQ.Client;

namespace SmartFridgeManagerAPI.Domain.Common;

public abstract class BaseQueue<T> where T : class
{
    public abstract bool Exclusive { get; }
    public abstract bool AutoDelete { get; }
    public abstract Dictionary<string, object>? Arguments { get; }
    public abstract string Exchange { get; }
    public abstract string RoutingKey { get; }
    public abstract IBasicProperties? BasicProperties { get; }
    public abstract string Queue { get; }
    public abstract bool Durable { get; }
    public abstract T Body { get; }
}
