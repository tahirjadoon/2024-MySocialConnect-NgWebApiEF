using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Data;
using MSC.Core.DB.Entities;

namespace MSC.Core.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    public async Task<AppUser> GetUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user;
    }

    public async Task<AppUser> GetUserAsync(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(userName));
        return user;
    }

    public async Task<AppUser> GetUserAsync(Guid guid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Guid.ToString() == guid.ToString());
        return user;
    }

    
}
