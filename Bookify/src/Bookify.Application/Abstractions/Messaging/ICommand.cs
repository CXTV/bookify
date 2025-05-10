using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Application.Abstractions.Messaging;

public interface IBaseCommand
{
}

//无参数只有返回值
public interface ICommand : IRequest<Result>, IBaseCommand
{
}

//有参数有返回值
public interface ICommand<TReponse> : IRequest<Result<TReponse>>, IBaseCommand
{
}
