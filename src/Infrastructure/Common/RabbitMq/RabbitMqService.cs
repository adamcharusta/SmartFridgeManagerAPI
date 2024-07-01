using System.Text;
using RabbitMQ.Client;
using SmartFridgeManagerAPI.Domain.Queues.Common;

namespace SmartFridgeManagerAPI.Infrastructure.Common.RabbitMq;

public class RabbitMqService : IRabbitMqService
{
    private readonly IModel _channel;
    private readonly IConnection _connection;

    public RabbitMqService(RabbitMqSettings settings)
    {
        ConnectionFactory factory = new()
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void BasicPublish<T>(BaseQueue<T> queue) where T : class
    {
        try
        {
            _channel.QueueDeclare(
                queue.Queue,
                queue.Durable,
                queue.Exclusive,
                queue.AutoDelete,
                queue.Arguments
            );

            string message = JsonConvert.SerializeObject(queue.Body);
            byte[] body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                queue.Exchange,
                queue.RoutingKey,
                queue.BasicProperties,
                body);

            queue.LogEvent();
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error while publish message");
            throw;
        }
    }

    ~RabbitMqService()
    {
        if (_channel.IsOpen)
        {
            _channel?.Close();
            _channel?.Dispose();
        }

        if (_connection.IsOpen)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
