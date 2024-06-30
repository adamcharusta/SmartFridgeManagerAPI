using RabbitMQ.Client;
using SmartFridgeManagerAPI.Domain.Queues.Common;

namespace SmartFridgeManagerAPI.Domain.Queues;

public class Email
{
    public required string Address { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
}

public class EmailQueue(string address, string subject, string body) : BaseQueue<Email>
{
    public override bool Durable => false;
    public override string Queue => "email";
    public override bool Exclusive => false;
    public override bool AutoDelete => false;
    public override Dictionary<string, object>? Arguments => null;
    public override string Exchange => string.Empty;
    public override string RoutingKey => "email";
    public override IBasicProperties? BasicProperties => null;
    public override Email Body => new() { Address = address, Subject = subject, Body = body };
}
