using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Extensions;

namespace MSC.Core.Mappers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        Map_AppUser_To_UserDto();
        Map_Photo_To_PhotoDto();
        Map_AppUser_To_LoggedInUserDto();
        Map_UserRegisterDto_To_AppUser();
        Map_MemberUpdateDto_To_AppUser();
    }

    #region Mappers start

    private void Map_AppUser_To_UserDto()
    {
        //same name propertirs will be automatically mapped
        //Age will also get automatically mapped since source has GetAge, the keyword Age are the same
        //PhotoUrl we'll need to map manually. will pick the url where isMain is true. Do check for null. 
        //  ***Hint: An expression tree lambda may not contain a null propagating operator.
        //  so use a function intead
        CreateMap<AppUser, UserDto>()
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => PickMainUrl(src.Photos)))
        .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge())) //src.GetAge()
        ;
    }

    private void Map_Photo_To_PhotoDto()
    {
        CreateMap<Photo, PhotoDto>();
    }

    private void Map_AppUser_To_LoggedInUserDto()
    {
        CreateMap<AppUser, LoggedInUserDto>()
        .ForMember(dest => dest.MainPhotoUrl, opt => opt.MapFrom(src => PickMainUrl(src.Photos)))
        ;
    }

    private void Map_UserRegisterDto_To_AppUser()
    {
        CreateMap<UserRegisterDto, AppUser>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.ToLowerInvariant()))
        ;
    }

    private void Map_MemberUpdateDto_To_AppUser()
    {
        CreateMap<MemberUpdateDto, AppUser>();
    }

    #endregion Mappers end

    #region Helper functions

    private static string PickMainUrl(IEnumerable<Photo> photos)
    {
        if(photos == null || !photos.Any()) 
            return string.Empty;
        var url = photos.FirstOrDefault(x => x.IsMain)?.Url ?? string.Empty;
        return url;
    }

    #endregion
}
