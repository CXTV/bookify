using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;

public sealed class User : Entity
{

    //添加角色字段（属性）
    private readonly List<Role> _roles = new();


    private User(Guid id, FirstName firstName, LastName lastName, Email email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    private User()
    {
    }

    public FirstName FirstName { get; private set; }

    public LastName LastName { get; private set; }

    public Email Email { get; private set; }

    //身份识别ID
    public string IdentityId { get; private set; } = string.Empty;

    //获取角色
    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    //由于设置了private set，所以只能在类内部修改,然后外部通过Create方法创建对象
    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email);

        //添加领域事件
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        //添加角色信息
        user._roles.Add(Role.Registered);

        return user;
    }

    //设置身份识别ID
    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}
