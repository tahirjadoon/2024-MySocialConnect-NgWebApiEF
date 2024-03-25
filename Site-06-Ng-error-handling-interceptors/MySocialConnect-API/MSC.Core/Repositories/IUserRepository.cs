using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;

namespace MSC.Core.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();

    Task<AppUser> GetUserRawAsync(string userName);

    Task<AppUser> GetUserAsync(int id);

    Task<AppUser> GetUserAsync(string userName);

    Task<AppUser> GetUserAsync(Guid guid);

    Task<bool> UserExists(string userName);

    Task<bool> RegisterUserAsync(AppUser user);

    Task<bool> SaveAllAsync();
}
