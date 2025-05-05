using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Domain.Abstractions;

//抽象类
public abstract class Entity
{
    //构造函数，只有子类可以调用
    protected Entity(Guid id)
    {
        Id = id;
    }
    //init属性，只有在对象初始化时可以赋值，一旦对象构造完成，就无法再改变这个值
    public Guid Id { get; init; }
}