using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MSC.Core.Constants;
using MSC.Core.DB.Data;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
using MSC.Core.Extensions;

namespace MSC.Core.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #region Update and SaveAll

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    /*
    Removed after UnitOfWork Implemention
    public async Task<bool> SaveAllAsync()
    {
        //make sure that the changes have been saved
        var isSave = await _context.SaveChangesAsync() > 0;
        return isSave;
    }
    */

    #endregion Update and SaveAll

    #region Get Users

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        //var users = await _context.Users.ToListAsync();
        var users = await _context.Users.Include(p => p.Photos).ToListAsync();                        
        return users;
    }

    //same as above "GetUsersAsync" but using auto mapper queryable extensions
    //with pagination changed the signature
    //public async Task<IEnumerable<UserDto>> GetUsersAMQEAsync()
    public async Task<PagedList<UserDto>> GetUsersAMQEAsync(UsersSearchParamDto userParams, Guid userGuid)
    {
        /*
         var users = await _context.Users
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
        return users;
        */
        var query = _context.Users.AsQueryable();
        //apply filters
        query = query.Where(u =>u.Guid != userGuid);
        if(!string.IsNullOrWhiteSpace(userParams.Gender))
            query = query.Where(u => u.Gender == userParams.Gender);
        
        var minDob = userParams.MaxAge.CalculateMinDob();
        var maxDob = userParams.MinAge.CalculateMaxDob();
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        if (!string.IsNullOrWhiteSpace(userParams.OrderBy))
        {
            //the new switch statement. _ is the default
            query = userParams.OrderBy switch
            {
                DataConstants.Created => query.OrderByDescending(u => u.CreatedOn),
                _ => query.OrderByDescending(u => u.LastActive)
            };
        }

        var finalQuery = query
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking() //EF will not track
                    ;

        //user pagedList class
        var pageList = await PagedList<UserDto>.CreateAsync(finalQuery, userParams.PageNumber, userParams.PageSize);

        return pageList;
    }

    public async Task<AppUser> GetUserRawAsync(string userName, bool includePhotos = false)
    {
        if(string.IsNullOrWhiteSpace(userName))
            throw new ValidationException("Invalid user name");
        
        AppUser user = null;
        if(!includePhotos)
            user = await _context.Users.SingleOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        else
        {
            user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        }
        return user;
    }

    public async Task<AppUser> GetUserAsync(int id, bool includePhotos = false)
    {
        AppUser user  = null;
        if(includePhotos)
        {
            user = await _context.Users
                                .Include(p => p.Photos)
                                .FirstOrDefaultAsync(x => x.Id == id);
        }
        else{
            user = await _context.Users.FindAsync(id);
        }
        
        return user;
    }

    //same as above "GetUserAsync" but using auto mapper queryable extensions
    public async Task<UserDto> GetUserAMQEAsync(int id)
    {
        var user = await _context.Users
                    .Where(x => x.Id == id)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
        return user;
    }

    public async Task<AppUser> GetUserAsync(string userName)
    {
        //var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(userName));
        var user = await _context.Users
                                .Include(p => p.Photos)
                                .FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        return user;
    }

    //same as above "GetUserAsync" but using auto mapper queryable extensions
    public async Task<UserDto> GetUserAMQEAsync(string userName)
    {
        var user = await _context.Users
                    .Where(x => x.UserName.ToLower() == userName.ToLower())
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
        return user;
    }

    public async Task<AppUser> GetUserAsync(Guid guid)
    {
        //var user = await _context.Users.FirstOrDefaultAsync(x => x.Guid.ToString() == guid.ToString());
        //var user = await _context.Users.FirstOrDefaultAsync(x => x.Guid == guid);
        var user = await _context.Users
                                .Include(p => p.Photos)
                                .FirstOrDefaultAsync(x => x.Guid == guid);
        return user;
    }

    //same as above "GetUserAsync" but using auto mapper queryable extensions
    public async Task<UserDto> GetUserAMQEAsync(Guid guid)
    {
        var user = await _context.Users
                    .Where(x => x.Guid == guid)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
        return user;
    }

    public async Task<string> GetUserGenderAsync(Guid guid)
    {
        var gender = await _context.Users.Where(x => x.Guid == guid).Select(x => x.Gender).FirstOrDefaultAsync();
        return gender;
    }

    #endregion Get Users
    
    #region Register

    public async Task<bool> UserExists(string userName)
    {
        return await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
    }

    /*changed the signature after UnitOfWork Implementation
    public async Task<bool> RegisterUserAsync(AppUser appUser)
    {
        if (appUser == null)
            throw new ValidationException("Invalid user");

        _context.Users.Add(appUser);
        var isSave = await SaveAllAsync();
        return isSave;
    }
    */
    public void RegisterUser(AppUser appUser)
    {
        if (appUser == null)
            throw new ValidationException("Invalid user");

        _context.Users.Add(appUser);
    }

    #endregion Register - Update - SaveAll
}
