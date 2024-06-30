using SmartFridgeManagerAPI.Domain.Common;

namespace SmartFridgeManagerAPI.Infrastructure.Services;

public interface IRabbitMqService
{
    void BasicPublish<T>(BaseQueue<T> queue) where T : class;
}
