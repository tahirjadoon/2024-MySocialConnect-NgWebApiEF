using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.ExceptionCustom;
using MSC.Core.Extensions;
using MSC.Core.Repositories;
using MSC.Core.Services;

namespace MSC.Core.BusinessLogic;

public class UserBusinessLogic : IUserBusinessLogic
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;

    public UserBusinessLogic(IUserRepository userRepo, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
    }

    #region Get Users
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var users = await _userRepo.GetUsersAsync();
        return users;
    }

    public async Task<AppUser> GetUserRawAsync(string userName)
    {
        if(string.IsNullOrWhiteSpace(userName)) 
            throw new ValidationException("User name missing");
        
        var user = await _userRepo.GetUserRawAsync(userName);

        return user;
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
    
    #endregion Get Users 

    #region  Register - Login - UserExists

    public async Task<bool> UserExists(string userName)
    {
        if(string.IsNullOrWhiteSpace(userName)) 
            throw new ValidationException("User name missing");

        return await _userRepo.UserExists(userName);
    }

    public async Task<LoggedInUserDto> RegisterUserAsync(UserRegisterDto registerUser)
    {
        if (registerUser == null)
            throw new ValidationException("Invalid user");
        if(string.IsNullOrWhiteSpace(registerUser.UserName)) 
            throw new ValidationException("User name missing");
        if(string.IsNullOrWhiteSpace(registerUser.Password))
            throw new ValidationException("Password is missing");
        
        if(await UserExists(registerUser.UserName))
            throw new ValidationException("Username already taken");

        //hash the password, it will give back hash and salt key
        var hashSalt = registerUser.Password.ComputeHashHmacSha512();
        if(hashSalt == null)
            throw new ValidationException("Unable to handle provided password");
        
        //create app user to save
        var appUser = new AppUser();
        appUser.UserName = registerUser.UserName.ToLower();
        appUser.PasswordHash = hashSalt.Hash;
        appUser.PasswordSalt = hashSalt.Salt;

        var isRegister = await _userRepo.RegisterUserAsync(appUser);
        if(!isRegister)
            throw new DataFailException("User not registerd");

        var returnUser = await _userRepo.GetUserRawAsync(registerUser.UserName);
        if(returnUser == null)
            throw new DataFailException("Something went wrong. No user found!");

        var loggedInUser = new LoggedInUserDto();
        loggedInUser.UserName = returnUser.UserName;
        loggedInUser.Guid = returnUser.Guid;
        loggedInUser.Token = _tokenService.CreateToken(returnUser);

        return loggedInUser;
    }

    public async Task<LoggedInUserDto> LoginAsync(LoginDto login)
    {
        if (login == null)
            throw new ValidationException("Login info missing");

        var user = await _userRepo.GetUserRawAsync(login.UserName);
        if (user == null || user.PasswordSalt == null || user.PasswordHash == null)
            throw new UnauthorizedAccessException("Either username or password is wrong");

         //password is hashed in db. Hash login password and check against the DB one
        var hashKeyLogin = login.Password.ComputeHashHmacSha512(user.PasswordSalt);
        if (hashKeyLogin == null)
            throw new UnauthorizedAccessException("Either username or password is wrong");

        //both are byte[]
        if (!hashKeyLogin.Hash.AreEqual(user.PasswordHash))
            throw new UnauthorizedAccessException("Either username or password is wrong");

        var loggedInUser = new LoggedInUserDto();
        loggedInUser.UserName = user.UserName;
        loggedInUser.Guid = user.Guid;
        loggedInUser.Token = _tokenService.CreateToken(user);

        return loggedInUser;
    }

    #endregion Register
}
