using System;

namespace MSC.Core.Dtos;

public class LoggedInUserDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public Guid Guid { get; set; }
    public string Token { get; set; } 
    public string DisplayName {get; set;}
    public string MainPhotoUrl {get; set;}
    public string Gender { get; set; }
}
