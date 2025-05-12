namespace Bookify.Infrastructure.Outbox;
public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Content = content;
        Type = type;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }
    //消息类型
    public string Type { get; init; }
    //消息内容
    public string Content { get; init; }
    //被处理的时间，null则表示未处理
    public DateTime? ProcessedOnUtc { get; init; }
    //记录错误信息
    public string? Error { get; init; }
}

