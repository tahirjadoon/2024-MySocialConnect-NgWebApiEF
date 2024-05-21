using System.Collections.Generic;
using System.Linq;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Extensions;
using MSC.Core.Services;

namespace MSC.Core.Mappers;

public static class ManualMapperExtensions
{
    public static LoggedInUserDto ManualMapToLoggedInUserDto(this AppUser user, ITokenService tokenService)
    {
        if(user == null)
            return null;

        var loggedInUser = new LoggedInUserDto();
        loggedInUser.Id = user.Id;
        loggedInUser.UserName = user.UserName;
        loggedInUser.Guid = user.Guid;
        loggedInUser.DisplayName = user.DisplayName;
        loggedInUser.Gender = user.Gender;
        loggedInUser.MainPhotoUrl = user.Photos.ManualGetMainPhotoUrl();
        //IR_REFACTOR :  CreateToken is async call now so commented it. Not used any more
        //loggedInUser.Token = tokenService.CreateToken(user);

        return loggedInUser;
    }

    public static string ManualGetMainPhotoUrl(this IEnumerable<Photo> photos)
    {
        var mainPhotoUrl = "";
        if(photos != null && photos.Any(x => x.IsMain == true)){
            var main = photos.FirstOrDefault(x => x.IsMain == true);
            if(main != null)
                mainPhotoUrl = main.Url;
        }
        return mainPhotoUrl;
    }

    public static IEnumerable<UserDto> ManualMapUsers(this IEnumerable<AppUser> users)
    {
        if(users == null || !users.Any())
            return null;

        List<UserDto> usersDto = new List<UserDto>();
        foreach(var user in users)
        {
            var userDto = user.ManualMapUser();
            if(userDto != null);
                usersDto.Add(userDto);
        }

        if(!usersDto.Any())
            return null;

        return usersDto;
    }

    public static UserDto ManualMapUser(this AppUser user)
    {
        if(user == null)
            return null;

        var userDto = new UserDto()
        {
            Id = user.Id,
            GuId = user.Guid,
            UserName = user.UserName,
            PhotoUrl = user.Photos.ManualGetMainPhotoUrl(),
            Age = user.DateOfBirth.CalculateAge(), //user.GetAge() removed from the appuser
            DisplayName = user.DisplayName,
            Gender = user.Gender,
            Introduction = user.Introduction,
            LookingFor = user.LookingFor, 
            Interests = user.Interests, 
            City = user.City,
            Country = user.Country,
            LastActive = user.LastActive,
            CreatedOn = user.CreatedOn,
            UpdatedOn = user.UpdatedOn, 
            Photos = user.Photos.ManualMapPhotos()
        };

        return userDto;
    }

    public static List<PhotoDto> ManualMapPhotos(this List<Photo> photos)
    {
        if(photos == null || !photos.Any())
            return null;

        var photoDtos = new List<PhotoDto>();

        foreach(var photo in photos)
        {
            var photoDto = photo.ManualMapPhoto();
            if(photoDto != null)
                photoDtos.Add(photoDto);
        }

        if(!photoDtos.Any())
            return null;

        return photoDtos;
    }

    public static PhotoDto ManualMapPhoto(this Photo photo)
    {
        if(photo == null)
            return null;

        var photoDto = new PhotoDto()
        {
            Id = photo.Id,
            Url = photo.Url,
            IsMain = photo.IsMain,
            PublicId = photo.PublicId
        };
        return photoDto;
    }
}
