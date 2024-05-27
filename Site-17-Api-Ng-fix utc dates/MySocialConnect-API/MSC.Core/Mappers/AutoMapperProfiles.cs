using System;
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
        Map_Message_to_MessageDto();

        //all dates to be passed back as UTC. EF when pulling dates is not adding Z to the dates
        //this will handle all datetims that are not optional
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        //for optional dates
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
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

    private void Map_Message_to_MessageDto()
    {
        CreateMap<UserMessage, MessageDto>()
        .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => PickMainUrl(src.Sender.Photos)))
        .ForMember(dest => dest.SenderGuid, opt => opt.MapFrom(src => src.Sender.Guid))
        .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => PickMainUrl(src.Recipient.Photos)))
        .ForMember(dest => dest.RecipientGuid, opt => opt.MapFrom(src => src.Recipient.Guid))
        ;
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
