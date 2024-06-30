using SmartFridgeManagerAPI.Domain.Queues.Common;

namespace SmartFridgeManagerAPI.Infrastructure.Common.Services;

public interface IRabbitMqService
{
    void BasicPublish<T>(BaseQueue<T> queue) where T : class;
}
