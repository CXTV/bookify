using System.Buffers;
using System.Text.Json;
using Bookify.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    /// <summary>
    /// 从缓存中获取一个指定类型的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">缓存的key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await _cache.GetAsync(key, cancellationToken);
        //将字节数组反序列化为指定类型
        return bytes is null ? default : Deserialize<T>(bytes);
    }

    /// <summary>
    /// 将一个对象存储到缓存中
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="key"></param>
    /// <param name="value">需要缓存的值</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        return _cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(key, cancellationToken);

    /// <summary>
    /// 反序列化字节数组为指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }

    /// <summary>
    /// 序列化对象为字节数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();

        using var writer = new Utf8JsonWriter(buffer);

        JsonSerializer.Serialize(writer, value);

        return buffer.WrittenSpan.ToArray();
    }
}
