using System;

namespace MSC.Core;

public class UserClaimGetDto
{
    public int Id {get; set; }
    public string UserName { get; set; }
    public Guid Guid { get; set; }
    public string DisplayName { get; set; }

    public bool HasUserName => !string.IsNullOrWhiteSpace(UserName);
    public bool HasGuid => Guid != Guid.Empty;
    public bool HasId => Id > 0;
}
