using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Repositories;

namespace MSC.Core.BusinessLogic;

public class UserBusinessLogic : IUserBusinessLogic
{
    private readonly IUserRepository _userRepo;

    public UserBusinessLogic(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var users = await _userRepo.GetUsersAsync();
        return users;
    }

    public async Task<AppUser> GetUserAsync(int id)
    {
        var user = await _userRepo.GetUserAsync(id);
        return user;
    }

    public async Task<AppUser> GetUserAsync(string userName)
    {
        var user = await _userRepo.GetUserAsync(userName);
        return user;
    }

    public async Task<AppUser> GetUserAsync(Guid guid)
    {
        var user = await _userRepo.GetUserAsync(guid);
        return user;
    }
    
}
