
using System.Collections.Generic;
using MSC.Core.DB.Entities;

namespace MSC.Core.Constants;

//IR_REFACTOR
public class SiteIdentityConstants
{
    public const string Role_Member = "Member";
    public const string Role_Admin = "Admin";
    public const string Role_Moderator = "Moderator";

    public static List<AppRole> SiteRoles = new List<AppRole>{
        new AppRole{ Name = SiteIdentityConstants.Role_Member},
        new AppRole{ Name = SiteIdentityConstants.Role_Admin},
        new AppRole{ Name = SiteIdentityConstants.Role_Moderator}
    };

    public static List<AppRole> SiteAdminModeratorRoles = new List<AppRole>{
        new AppRole{ Name = SiteIdentityConstants.Role_Admin},
        new AppRole{ Name = SiteIdentityConstants.Role_Moderator}
    };

    public const string AuthPolicy_Admin = "AuthPolicy_Admin";
    public const string AuthPolicy_Moderator_Photos = "AuthPolicy_Moderator_Photos";
}
