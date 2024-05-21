using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MSC.Core.DB.Entities;

//IR_REFATCOR
//Many to Many relationship with AppUser where
//A user can have may roles
//and a role can have many users
public class AppRole : IdentityRole<int>
{
    //navigation property to the join table
    public ICollection<AppUserRole> UserRoles {get; set;}
}
