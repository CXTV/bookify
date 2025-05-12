using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Caching;

//泛型接口：定义有有返回值的缓存，并且继承了ICachedQuery
public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;

//定义缓存接口，有key和过期时间
public interface ICachedQuery
{
    string CacheKey { get; }

    TimeSpan? Expiration { get; }
}
