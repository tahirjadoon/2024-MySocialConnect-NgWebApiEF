using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;

namespace MSC.Core.BusinessLogic;

public interface IUserBusinessLogic
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    //same as above "GetUsersAsync" but using auto mapper queryable extensions
    Task<IEnumerable<UserDto>> GetUsersAMQEAsync();

    Task<AppUser> GetUserRawAsync(string userName);

    Task<UserDto> GetUserAsync(int id);
    //same as above "GetUserAsync" but using auto mapper queryable extensions
    Task<UserDto> GetUserAMQEAsync(int id);

    Task<UserDto> GetUserAsync(string userName);
    //same as above "GetUserAsync" but using auto mapper queryable extensions
    Task<UserDto> GetUserAMQEAsync(string userName);

    Task<UserDto> GetUserAsync(Guid guid);
    //same as above "GetUserAsync" but using auto mapper queryable extensions
    Task<UserDto> GetUserAMQEAsync(Guid guid);

    Task<bool> UserExists(string userName);

    Task<LoggedInUserDto> LoginAsync(LoginDto login);

    Task<LoggedInUserDto> RegisterUserAsync(UserRegisterDto registerUser);

    Task<bool> UpdateUserAsync(MemberUpdateDto memberUpdateDto, UserClaimGetDto claims);

    Task<PhotoDto> AddPhotoAsync(IFormFile file, UserClaimGetDto claims);

    Task<BusinessResponse> DeletePhotoAsync(int photoId, UserClaimGetDto claims);

    Task<bool> SetPhotoMainAsync(int photoId, UserClaimGetDto claims);
}
