namespace Bookify.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    // 发送到消息队列的间隔时间
    public int IntervalInSeconds { get; init; }
    // 批量大小
    public int BatchSize { get; init; }
}
