using Microsoft.AspNetCore.Identity;

namespace MSC.Core.DB.Entities;

//IR_REFATCOR
//A joining table between AppUser and AppRole
//Used in both AppRole and and AppUser
//User <=> Role have Many to Many relationship
//A user can have many roles
//and a role can have many users
//so put iCollection of AppUSerRole in both AppRole and AppUser
public class AppUserRole : IdentityUserRole<int>
{
    public AppUser User { get; set; }
    public AppRole Role { get; set; }
}
