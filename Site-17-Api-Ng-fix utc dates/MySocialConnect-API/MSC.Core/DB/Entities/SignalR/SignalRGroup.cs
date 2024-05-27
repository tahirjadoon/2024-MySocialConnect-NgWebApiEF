using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSC.Core.DB.Entities.SignalR;

public class SignalRGroup
{
    /// <summary>
    /// Empty constructor is needed for the EF
    /// </summary>
    public SignalRGroup()
    {
    }

    public SignalRGroup(string groupName)
    {
        GroupName = groupName;
    }

    /// <summary>
    /// GroupName is the unique key and it will be indexed as well
    /// </summary>
    [Key]
    public string GroupName { get; set; }

    public ICollection<SignalRConnection> Connections { get; set; } = new List<SignalRConnection>();
}
