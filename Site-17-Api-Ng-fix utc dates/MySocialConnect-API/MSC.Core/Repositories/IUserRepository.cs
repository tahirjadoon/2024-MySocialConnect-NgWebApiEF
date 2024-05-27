using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;

namespace MSC.Core.Repositories;

public interface IUserRepository
{
    void Update(AppUser user);
    
    Task<bool> SaveAllAsync();

    Task<IEnumerable<AppUser>> GetUsersAsync();

    //same as above "GetUsersAsync" but using auto mapper queryable extensions
    //with pagination changed the signature
    //Task<IEnumerable<UserDto>> GetUsersAMQEAsync();
    Task<PagedList<UserDto>> GetUsersAMQEAsync(UsersSearchParamDto userParams, Guid userGuid);

    Task<AppUser> GetUserRawAsync(string userName, bool includePhotos = false);

    Task<AppUser> GetUserAsync(int id, bool includePhotos = false);
    //same as above "GetUsersAsync" but using auto mapper queryable extensions
    Task<UserDto> GetUserAMQEAsync(int id);

    Task<AppUser> GetUserAsync(string userName);
    //same as above "GetUserAsync" but using auto mapper queryable extensions
    Task<UserDto> GetUserAMQEAsync(string userName);

    Task<AppUser> GetUserAsync(Guid guid);
    //same as above "GetUserAsync" but using auto mapper queryable extensions
    Task<UserDto> GetUserAMQEAsync(Guid guid);

    Task<bool> UserExists(string userName);

    Task<bool> RegisterUserAsync(AppUser user);
}
