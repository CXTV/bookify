using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using FluentValidation;
using MediatR;
using ValidationException = Bookify.Application.Exceptions.ValidationException;

namespace Bookify.Application.Abstractions.Behaviors;

//只对继承IBaseCommand接口，进行处理
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> //IPipelineBehavior 是 MediatR 提供的接口，在请求发送到 handler 之前或之后插入额外逻辑
    where TRequest : IBaseCommand //指定 TRequest 必须实现 IBaseCommand 接口
{
    //1.获取所有的验证器
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    //2.构造函数注入所有的验证器
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    //3.IPipelineBehavior核心方法
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        //4.创建一个验证上下文对象，将请求传入给 FluentValidation 使用
        var context = new ValidationContext<TRequest>(request);
        //5.使用 LINQ 查询验证器，获取验证结果
        var validationErrors = _validators
            .Select(validator => validator.Validate(context))
            .Where(validationResult => validationResult.Errors.Any())
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage))
            .ToList();
        //6.如果验证失败，抛出 ValidationException 异常
        if (validationErrors.Any())
        {
            throw new ValidationException(validationErrors);
        }

        return await next();
    }
}
