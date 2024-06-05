using System;

namespace MSC.Core.Dtos;

public class PhotoForApprovalDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string UserName { get; set; }
    public int UserId { get; set; }
    public Guid UserGuid { get; set; }
    public bool IsApproved { get; set; }
}
