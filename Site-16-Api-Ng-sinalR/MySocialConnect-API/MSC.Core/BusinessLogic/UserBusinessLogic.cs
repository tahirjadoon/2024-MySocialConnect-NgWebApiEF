using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSC.Core.Constants;
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
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UserBusinessLogic(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, 
                            IUserRepository userRepo, ITokenService tokenService, IMapper mapper, 
                            IPhotoService photoService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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
    public async Task<AppUser> GetUserRawAsync(string userName, bool includePhotos = false)
    {
        if(string.IsNullOrWhiteSpace(userName)) 
            throw new ValidationException("User name missing");
        
        var user = await _userRepo.GetUserRawAsync(userName, includePhotos);

        return user;
    }

    public async Task<AppUser> GetUserRawAsync(int id, bool includePhotos = false)
    {
        if(id <= 0)
            throw new ValidationException("User id missing");

        var user = await _userRepo.GetUserAsync(id, includePhotos);

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

        //IR_REFACTOR : use the user manager
        //return await _userRepo.UserExists(userName);
        return await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
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

        //IR_REFACTOR
        /*
        //hash the password, it will give back hash and salt key
        var hashSalt = registerUser.Password.ComputeHashHmacSha512();
        if(hashSalt == null)
            throw new ValidationException("Unable to handle provided password");
        */

        //create app user to save
        /*
        var appUser = new AppUser();
        appUser.UserName = registerUser.UserName.ToLower();
        */
        var appUser = _mapper.Map<AppUser>(registerUser);
        ////IR_REFATCOR: removed these properties
        //appUser.PasswordHash = hashSalt.Hash;
        //appUser.PasswordSalt = hashSalt.Salt;

        //IR_REFACTOR
        /*
        var isRegister = await _userRepo.RegisterUserAsync(appUser);
        if(!isRegister)
            throw new DataFailException("User not registerd");
        */
        var result = await _userManager.CreateAsync(appUser, registerUser.Password);
        if(!result.Succeeded)
            throw new DataFailException(string.Join(", ", result.Errors));

        //add the user to the member role
        var roleResult = await _userManager.AddToRoleAsync(appUser, SiteIdentityConstants.Role_Member);
        if(!roleResult.Succeeded)
            throw new DataFailException(roleResult.Errors.ToString());        
        
        var returnUser = await _userRepo.GetUserRawAsync(registerUser.UserName, includePhotos: true);
        if(returnUser == null)
            throw new DataFailException("Something went wrong. No user found!");

        //var loggedInUser = returnUser.ManualMapToLoggedInUserDto(_tokenService);;
        var loggedInUser = _mapper.Map<LoggedInUserDto>(returnUser);
        loggedInUser.Token = await _tokenService.CreateToken(returnUser);
        return loggedInUser;
    }

    public async Task<LoggedInUserDto> LoginAsync(LoginDto login)
    {
        if (login == null)
            throw new ValidationException("Login info missing");
        
        //IR_REFATCOR:
        //var user = await _userRepo.GetUserRawAsync(login.UserName, includePhotos: true);
        var user = await _userManager.Users
                                    .Include(p => p.Photos)
                                    .SingleOrDefaultAsync(x => x.UserName == login.UserName.ToLower());

        ////IR_REFATCOR: removed these properties
        //if (user == null || user.PasswordSalt == null || user.PasswordHash == null)
        //    throw new UnauthorizedAccessException("Either username or password is wrong");
        if(user == null)
            throw new UnauthorizedAccessException("Either username or password is wrong");

        //password is hashed in db. Hash login password and check against the DB one
        ////IR_REFATCOR: removed these properties
        /*
        var hashKeyLogin = login.Password.ComputeHashHmacSha512(user.PasswordSalt);
        if (hashKeyLogin == null)
            throw new UnauthorizedAccessException("Either username or password is wrong");

        //both are byte[]
        if (!hashKeyLogin.Hash.AreEqual(user.PasswordHash))
            throw new UnauthorizedAccessException("Either username or password is wrong");
        */
        var result = await _userManager.CheckPasswordAsync(user, login.Password);        
        if (!result)
            throw new UnauthorizedAccessException("Either username or password is wrong");
        //mapping via manual user mapper
        //var loggedInUser = user.ManualMapToLoggedInUserDto(_tokenService);
        var loggedInUser = _mapper.Map<LoggedInUserDto>(user);
        loggedInUser.Token = await _tokenService.CreateToken(user);
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

    #region Roles and Moderation

    public async Task<IEnumerable<object>> GetUSersWithRoles()
    {
        //get the users, include UserRoles and then Role
        //return an annonamous object
        //exclude admin user
        var users = await _userManager.Users 
                        //.Include(r => r.UserRoles)
                        //.ThenInclude(r => r.Role)
                        //.Where(u => u.UserName.ToLower() != "admin")
                        .OrderBy(u => u.DisplayName)
                        .Select(u => new {
                            u.Id,
                            UserName = u.UserName,
                            DisplayName = u.DisplayName, 
                            GuId = u.Guid, 
                            Roles = u.UserRoles.Select(r => r.Role.Name).OrderBy(x => x).ToList()
                        })
                        .ToListAsync();
        return users;
    }

    public async Task<BusinessResponse> EditRolesForUser(int adminUSerId, Guid userToUpdate, IEnumerable<string> roles)
    {
        //check user
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Guid == userToUpdate);
        if(user == null)
            return new BusinessResponse(HttpStatusCode.NotFound, "User not found to update");
        
        //check roles to update
        if(roles == null || !roles.Any())
            return new BusinessResponse(HttpStatusCode.BadRequest, "No roles passed to update");

        //get the site roles 
        var siteRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        //check roles to update are in siteRoles
        var notInSiteRoles = roles.Where(x => !siteRoles.Any(y => y == x)).ToList();
        if(notInSiteRoles != null && notInSiteRoles.Any())
            return new BusinessResponse(HttpStatusCode.BadRequest, $"Passed role(s) not in list {string.Join(",", notInSiteRoles)}");

        //current user roles
        var userRoles = await _userManager.GetRolesAsync(user);

        //add the new roles only that do not below to user currently 
        var resultAdd = await _userManager.AddToRolesAsync(user, roles.Except(userRoles));        
        if(!resultAdd.Succeeded)
            return new BusinessResponse(HttpStatusCode.BadRequest, "Failed to add the roles");

        //remove the roles as since the user may have removed some. Above is oly adding new ones
        var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(roles));
        if(!removeResult.Succeeded)
            return new BusinessResponse(HttpStatusCode.BadRequest, "Failed to remove roles");

        //pick new roles 
        var currentRoles = await _userManager.GetRolesAsync(user);

        return new BusinessResponse(HttpStatusCode.OK, "Roles updates successfully!", currentRoles);
    }

    #endregion Roles and Moderation
}
