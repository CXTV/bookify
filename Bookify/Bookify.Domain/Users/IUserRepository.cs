using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bookify.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(User user);
}