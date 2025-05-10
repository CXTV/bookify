using Bookify.Domain.Users;

namespace Bookify.Application.Abstractions.Authentications;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);
}
