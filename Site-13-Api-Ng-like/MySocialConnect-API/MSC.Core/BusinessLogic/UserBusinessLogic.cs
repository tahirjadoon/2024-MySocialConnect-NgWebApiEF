using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
using MSC.Core.ExceptionCustom;
using MSC.Core.Extensions;
using MSC.Core.Mappers;
using MSC.Core.Repositories;
using MSC.Core.Services;

namespace MSC.Core.BusinessLogic;

public class UserBusinessLogic : IUserBusinessLogic
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UserBusinessLogic(IUserRepository userRepo, ITokenService tokenService, IMapper mapper, IPhotoService photoService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _mapper = mapper;
        _photoService = photoService;
    }

    #region Get Users
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        //return IEnumerable AppUser
        var users = await _userRepo.GetUsersAsync();
        if(users == null || !users.Any())
            return null;
        //return users;
        
        //tie AppUser to the UserDto
        //var usersDto = users.ManualMapUsers();
        var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
        return usersDto;
    }

    //same as above "GetUsersAsync" but using auto mapper queryable extensions
    //signature changed due to pagination
    public async Task<PagedList<UserDto>> GetUsersAMQEAsync(UsersSearchParamDto userParams, Guid userGuid)
    {
        var users = await _userRepo.GetUsersAMQEAsync(userParams, userGuid);
        if(users == null || !users.Any()) return null;
        return users;
    }


    public async Task<AppUser> GetUserRawAsync(string userName)
    {
        if(string.IsNullOrWhiteSpace(userName)) 
            throw new ValidationException("User name missing");
        
        var user = await _userRepo.GetUserRawAsync(userName);

        return user;
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        var user = await _userRepo.GetUserAsync(id);
        //return user;

        if(user == null)
            return null;

        //var userDto = user.ManualMapUser();
        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    //same as above "GetUserAsync" but using auto mapper queryable extensions
    public async Task<UserDto> GetUserAMQEAsync(int id)
    {
        var user = await _userRepo.GetUserAMQEAsync(id);
        if(user == null) return null;
        return user;
    }

    public async Task<UserDto> GetUserAsync(string userName)
    {
        var user = await _userRepo.GetUserAsync(userName);
        //return user;

        if(user == null)
            return null;

        //var userDto = user.ManualMapUser();
        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    //same as above "GetUserAsync" but using auto mapper queryable extensions
    public async Task<UserDto> GetUserAMQEAsync(string userName)
    {
        var user = await _userRepo.GetUserAMQEAsync(userName);
        if(user == null) return null;
        return user;
    }

    public async Task<UserDto> GetUserAsync(Guid guid)
    {
        var user = await _userRepo.GetUserAsync(guid);
        //return user;

        if(user == null)
            return null;

        //var userDto = user.ManualMapUser();
        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    //same as above "GetUserAsync" but using auto mapper queryable extensions
    public async Task<UserDto> GetUserAMQEAsync(Guid guid)
    {
        var user = await _userRepo.GetUserAMQEAsync(guid);
        if(user == null) return null;
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
        /*
        var appUser = new AppUser();
        appUser.UserName = registerUser.UserName.ToLower();
        */
        var appUser = _mapper.Map<AppUser>(registerUser);
        appUser.PasswordHash = hashSalt.Hash;
        appUser.PasswordSalt = hashSalt.Salt;

        var isRegister = await _userRepo.RegisterUserAsync(appUser);
        if(!isRegister)
            throw new DataFailException("User not registerd");

        var returnUser = await _userRepo.GetUserRawAsync(registerUser.UserName, includePhotos: true);
        if(returnUser == null)
            throw new DataFailException("Something went wrong. No user found!");

        //var loggedInUser = returnUser.ManualMapToLoggedInUserDto(_tokenService);;
        var loggedInUser = _mapper.Map<LoggedInUserDto>(returnUser);
        loggedInUser.Token = _tokenService.CreateToken(returnUser);
        return loggedInUser;
    }

    public async Task<LoggedInUserDto> LoginAsync(LoginDto login)
    {
        if (login == null)
            throw new ValidationException("Login info missing");

        var user = await _userRepo.GetUserRawAsync(login.UserName, includePhotos: true);
        if (user == null || user.PasswordSalt == null || user.PasswordHash == null)
            throw new UnauthorizedAccessException("Either username or password is wrong");

         //password is hashed in db. Hash login password and check against the DB one
        var hashKeyLogin = login.Password.ComputeHashHmacSha512(user.PasswordSalt);
        if (hashKeyLogin == null)
            throw new UnauthorizedAccessException("Either username or password is wrong");

        //both are byte[]
        if (!hashKeyLogin.Hash.AreEqual(user.PasswordHash))
            throw new UnauthorizedAccessException("Either username or password is wrong");

        //mapping via manual user mapper
        //var loggedInUser = user.ManualMapToLoggedInUserDto(_tokenService);
        var loggedInUser = _mapper.Map<LoggedInUserDto>(user);
        loggedInUser.Token = _tokenService.CreateToken(user);
        return loggedInUser;
    }

    #endregion Register

    #region Updates

    public async Task<bool> UpdateUserAsync(MemberUpdateDto memberUpdateDto, UserClaimGetDto claims)
    {
        var user = await _userRepo.GetUserRawAsync(claims.UserName);
        if(user == null || user.Guid != claims.Guid)
            return false;

        //we have the mapped. current properties will be kept as is while other will be updated
        var updates = _mapper.Map(memberUpdateDto, user);

        //issue update
        _userRepo.Update(updates);

        //save updates
        if(await _userRepo.SaveAllAsync())
            return true;

        return false;
    }

    public async Task<PhotoDto> AddPhotoAsync(IFormFile file, UserClaimGetDto claims)
    {
        var appUser = await _userRepo.GetUserRawAsync(claims.UserName, includePhotos: true);
        if (appUser == null)
            throw new UnauthorizedAccessException("User not found");

        var result = await _photoService.AddPhotoAsync(file);
        if(result.Error != null)
            throw new DataFailException(result.Error?.Message ?? "Photo updload error");

        //success, build photo entity and save
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri, //set photo url
            PublicId = result.PublicId, //setup public id
            IsMain = appUser.Photos == null || !appUser.Photos.Any() //mark it active
        };

        //add the photo
        appUser.Photos.Add(photo);
        
        //_userRepo.Update(appUser);
        if(await _userRepo.SaveAllAsync())
            return _mapper.Map<PhotoDto>(photo);

        return null;
    }

    public async Task<BusinessResponse> DeletePhotoAsync(int photoId, UserClaimGetDto claims)
    {
        var appUser = await _userRepo.GetUserRawAsync(claims.UserName, includePhotos: true);
        if (appUser == null)
            throw new UnauthorizedAccessException("User not found");

        var response = new BusinessResponse();

        var photo = appUser.Photos?.FirstOrDefault(x => x.Id == photoId);
        if(photo == null)
        {
            response.HttpStatusCode = HttpStatusCode.NotFound;
            response.Message = "Photo not found";
            return response;
        }

        if(photo.IsMain)
        {
            response.HttpStatusCode = HttpStatusCode.BadRequest;
            response.Message = "You cannot delete your main photo";
            return response;
        }

        //delete from cludinary
        if(!string.IsNullOrWhiteSpace(photo.PublicId))
        {
            var result = await _photoService.DeletePhotoAync(photo.PublicId);
            if(result.Error != null)
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                response.Message = result.Error?.Message ?? "Unable to delete photo from service";
                return response;
            }
        }

        //remove from data base 
        appUser.Photos.Remove(photo);

        if(await _userRepo.SaveAllAsync())
        {
            response.HttpStatusCode = HttpStatusCode.OK;
            return response;
        }

        //here it is an error 
        response.HttpStatusCode = HttpStatusCode.BadRequest;
        response.Message = "Unable to delete photo";
        return response;
    }

    public async Task<bool> SetPhotoMainAsync(int photoId, UserClaimGetDto claims)
    {
        var appUser = await _userRepo.GetUserRawAsync(claims.UserName, includePhotos: true);
        if (appUser == null)
            throw new UnauthorizedAccessException("User not found");

        var photo = appUser.Photos?.FirstOrDefault(x => x.Id == photoId);
        if(photo == null)
            return false;
        
        if(photo.IsMain)
            throw new DataFailException("This is already your main photo");

        var currentMain = appUser.Photos?.FirstOrDefault( x=> x.IsMain == true);
        if(currentMain != null)
            currentMain.IsMain = false;
        
        photo.IsMain = true;

        if(await _userRepo.SaveAllAsync())
            return true;

        return false;
    }

    public async Task LogUserActivityAsync(int id)
    {
       if(id <= 0) return;

        //app user 
        var user = await _userRepo.GetUserAsync(id);
        if (user == null) return;

        //update the last active date 
        user.LastActive = DateTime.UtcNow;

        //update 
        await _userRepo.SaveAllAsync();
    }

    #endregion Updates
}
