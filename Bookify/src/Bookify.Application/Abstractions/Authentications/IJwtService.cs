using Bookify.Domain.Abstractions;

namespace Bookify.Application.Abstractions.Authentications;

public interface IJwtService
{
    Task<Result<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}
