using System;

namespace MSC.Core.Dtos;

public class LoggedInUserDto
{
    public string UserName { get; set; }
    public Guid Guid { get; set; }
    public string Token { get; set; } 
}
