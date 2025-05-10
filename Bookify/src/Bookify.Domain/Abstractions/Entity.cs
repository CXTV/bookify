namespace Bookify.Domain.Abstractions;

//抽象类
public abstract class Entity
{
    //存储该实体上已触发但尚未发布的领域事件
    private readonly List<IDomainEvent> _domainEvents = new();

    //构造函数，只有子类可以调用
    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
    }

    //init属性，只有在对象初始化时可以赋值，一旦对象构造完成，就无法再改变这个值
    public Guid Id { get; init; }

    //获取领域事件
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }
    //清除领域事件
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    //添加领域事件
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}