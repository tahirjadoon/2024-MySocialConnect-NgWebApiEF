using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

    #region Get Users

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    public async Task<AppUser> GetUserRawAsync(string userName)
    {
        if(string.IsNullOrWhiteSpace(userName))
            throw new ValidationException("Invalid user name");
        
        AppUser user = null;
        user = await _context.Users.SingleOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        return user;
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
        //var user = await _context.Users.FirstOrDefaultAsync(x => x.Guid.ToString() == guid.ToString());
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Guid == guid);
        return user;
    }

    #endregion Get Users
    
    #region Register - Update - SaveAll

    public async Task<bool> UserExists(string userName)
    {
        return await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
    }

    public async Task<bool> RegisterUserAsync(AppUser appUser)
    {
        if (appUser == null)
            throw new ValidationException("Invalid user");

        _context.Users.Add(appUser);
        var isSave = await SaveAllAsync();
        return isSave;
    }

    public async Task<bool> SaveAllAsync()
    {
        //make sure that the changes have been saved
        var isSave = await _context.SaveChangesAsync() > 0;
        return isSave;
    }

    #endregion Register - Update - SaveAll
}
