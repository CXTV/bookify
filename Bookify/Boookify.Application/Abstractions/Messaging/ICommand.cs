using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Application.Abstractions.Messaging;

public interface IBaseCommand
{
}
//无返回值的命令
public interface ICommand : IRequest<Result>, IBaseCommand
{
}
//返回一个TReponse的命令
public interface ICommand<TReponse> : IRequest<Result<TReponse>>, IBaseCommand
{
}
