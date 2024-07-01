using SmartFridgeManagerAPI.Domain.Queues.Common;

namespace SmartFridgeManagerAPI.Infrastructure.Common.RabbitMq;

public interface IRabbitMqService
{
    void BasicPublish<T>(BaseQueue<T> queue) where T : class;
}
