using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;

namespace MSC.Core.BusinessLogic;

public interface IUserBusinessLogic
{
    Task<IEnumerable<AppUser>> GetUsersAsync();

    Task<AppUser> GetUserRawAsync(string userName);

    Task<AppUser> GetUserAsync(int id);

    Task<AppUser> GetUserAsync(string userName);

    Task<AppUser> GetUserAsync(Guid guid);

    Task<bool> UserExists(string userName);

    Task<LoggedInUserDto> LoginAsync(LoginDto login);

    Task<LoggedInUserDto> RegisterUserAsync(UserRegisterDto registerUser);
}
