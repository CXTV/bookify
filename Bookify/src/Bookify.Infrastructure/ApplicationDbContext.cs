using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{

    //在序列化和反序列化过程中是否包含 .NET 类型信息。
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IDateTimeProvider _dateTimeProvider;


    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions options, IPublisher publisher, IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {

            AddDomainEventsAsOutboxMessages();
            //这样实现了原子性,存储数据和发布事件都在同一个事务中完成
            int result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    //发布领域事件
    //private async Task PublishDomainEventsAsync()
    //{
    //    var domainEvents = ChangeTracker
    //        .Entries<Entity>()
    //        .Select(entry => entry.Entity)
    //        .SelectMany(entity =>
    //        {
    //            IReadOnlyList<IDomainEvent> domainEvents = entity.GetDomainEvents();

    //            entity.ClearDomainEvents();

    //            return domainEvents;
    //        })
    //        .ToList();

    //    foreach (IDomainEvent domainEvent in domainEvents)
    //    {
    //        await _publisher.Publish(domainEvent);
    //    }
    //}

    //使用Outbox模式发布领域事件
    private void AddDomainEventsAsOutboxMessages()
    {
        //获取所有的领域事件
        var outboxMessages = ChangeTracker
            .Entries<Entity>()   //1. 获取所有Entity类型的实体
            .Select(entry => entry.Entity)  //2.取出实体对象
            .SelectMany(entity =>     //3. 
            {
                IReadOnlyList<IDomainEvent> domainEvents = entity.GetDomainEvents(); //4.获取所有领域事件

                entity.ClearDomainEvents();  //5.并清空

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(   // 6.将领域事件转换为OutboxMessage对象
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();
        //添加到数据库上下文,使用unit of work一起保存,完成原子性
        AddRange(outboxMessages);
    }
}
