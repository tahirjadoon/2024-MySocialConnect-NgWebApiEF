using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;

namespace MSC.Core.BusinessLogic;

public interface IUserBusinessLogic
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    //same as above "GetUsersAsync" but using auto mapper queryable extensions
    //signature changed due to pagination
    //Task<IEnumerable<UserDto>> GetUsersAMQEAsync();
    Task<PagedList<UserDto>> GetUsersAMQEAsync(UsersSearchParamDto userParams, Guid userGuid);

    Task<AppUser> GetUserRawAsync(string userName, bool includePhotos = false);

    Task<AppUser> GetUserRawAsync(int userName, bool includePhotos = false);

    Task<UserDto> GetUserAsync(int id);
    //same as above "GetUserAsync" but using auto mapper queryable extensions
    //for ignoring Query filter for the current user as setup via DataContext
    Task<UserDto> GetUserAMQEAsync(int id, UserClaimGetDto claims);

    Task<UserDto> GetUserAsync(string userName);
    //same as above "GetUserAsync" but using auto mapper queryable extensions
    //for ignoring Query filter for the current user as setup via DataContext
    Task<UserDto> GetUserAMQEAsync(string userName, UserClaimGetDto claims);

    Task<UserDto> GetUserAsync(Guid guid);
    //same as above "GetUserAsync" but using auto mapper queryable extensions
    //for ignoring Query filter for the current user as setup via DataContext
    Task<UserDto> GetUserAMQEAsync(Guid guid, UserClaimGetDto claims);

    Task<bool> UserExists(string userName);

    Task<LoggedInUserDto> LoginAsync(LoginDto login);

    Task<LoggedInUserDto> RegisterUserAsync(UserRegisterDto registerUser);

    Task<bool> UpdateUserAsync(MemberUpdateDto memberUpdateDto, UserClaimGetDto claims);

    Task<PhotoDto> AddPhotoAsync(IFormFile file, UserClaimGetDto claims);

    Task<BusinessResponse> DeletePhotoAsync(int photoId, UserClaimGetDto claims);

    Task<bool> SetPhotoMainAsync(int photoId, UserClaimGetDto claims);

    Task LogUserActivityAsync(int id);

    //from admin controller after IR_REFACTOR
    Task<IEnumerable<object>> GetUSersWithRoles();
    Task<BusinessResponse> EditRolesForUser(int adminUSerId, Guid userToUpdate, IEnumerable<string> roles);

    Task<string> GetUserGenderAsync(Guid guid);
}
