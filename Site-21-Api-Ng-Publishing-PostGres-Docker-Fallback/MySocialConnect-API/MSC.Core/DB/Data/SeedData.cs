using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSC.Core.Constants;
using MSC.Core.DB.Entities;
using MSC.Core.Extensions;

namespace MSC.Core.DB.Data;

//IR_REFACTOR
public class SeedData
{
    /// <summary>
    /// Broken after Identity use, Before IR_REFACTOR. Use new method SeedUsers that takes in UserManager
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [Obsolete]
    public static async Task SeedUsers(DataContext context)
    {
        //if users then do not seed new data 
        if(await context.Users.AnyAsync()) return;

        //seed data file location
        var location = System.AppDomain.CurrentDomain.BaseDirectory;
        var index = location.IndexOf("MSC.WebApi");
        var file = location.Substring(0, index-1);
        var fileFullPath = @$"{file}\MSC.Core\DB\Data\UserSeedData.json";

        var isFile = await Task.Run(() => File.Exists(fileFullPath));
        if(!isFile) return;

        //read file 
        var userData = await File.ReadAllTextAsync(fileFullPath);
        if(string.IsNullOrWhiteSpace(userData)) return;

        //Deserialize
        var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        if(users == null) return;

        //all the users will get the same password 
        var hashkey = "Password@1".ComputeHashHmacSha512();
        if(hashkey == null) return;

        //add the passwors hash, password salt and also convert the username to lower case
        foreach(var user in users){
            ////IR_REFATCOR: removed these properties
            /*
            user.PasswordHash = hashkey.Hash;
            user.PasswordSalt = hashkey.Salt;
            */
            user.UserName = user.UserName.ToLowerInvariant();

            context.Users.Add(user);
        }

        //save to database 
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// New method after Identity implementation
    /// </summary>
    /// <returns></returns>
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        //if users then do not seed new data 
        if(await userManager.Users.AnyAsync()) return;

        //seed data file location
        //var x = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //C:\\...\\MySocialConnect-API\\MSC.WebApi\\bin\\Debug\\net8.0
        var location = System.AppDomain.CurrentDomain.BaseDirectory;
        //var location = Directory.GetCurrentDirectory();
        //C:\\...\\MySocialConnect-API\\MSC.WebApi\\bin\\Debug\\net8.0\\

        var filePath = @"DB\Data\UserSeedData.json";
        //old way of getting to the file
        /*
        var index = location.IndexOf("MSC.WebApi");
        var file = location.Substring(0, index-1);
        var fileFullPath = @$"{file}\MSC.Core\{filePath}";
        */

        //Since file has been setup with CopyToOutputDirectory=Always, check MSC.Core.csproj for details.
        var fileFullPath = Path.Combine(location, filePath);

        var isFile = await Task.Run(() => File.Exists(fileFullPath));
        if(!isFile) return;

        //read file 
        var userData = await File.ReadAllTextAsync(fileFullPath);
        if(string.IsNullOrWhiteSpace(userData)) return;

        //Deserialize
        var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        if(users == null) return;

        //Roles
        List<AppRole> roles = SiteIdentityConstants.SiteRoles; //Helper property to get the roles as List<AppRole>
        foreach(var role in roles){
            await roleManager.CreateAsync(role);
        }

        //default password
        var defaultPassword = "Password@1";

        //convert the username to lower case
        foreach(var user in users){
            //with photo managment, make the first photo isApproved
            if(user.Photos != null && user.Photos.Any()){
                user.Photos.First().IsApproved = true;
            }

            user.UserName = user.UserName.ToLowerInvariant();
            await userManager.CreateAsync(user, defaultPassword); //will save as well. 
            //add the user to Member Role
            await userManager.AddToRoleAsync(user, SiteIdentityConstants.Role_Member);
        }

        //create Admin User and add Admin and Moderator roles
        var adminUser = new AppUser{
            UserName = "admin",
            DisplayName = "Admin",
            Gender = "male"
        };
        await userManager.CreateAsync(adminUser, defaultPassword);
        var adminModeratorRoles = SiteIdentityConstants.SiteAdminModeratorRoles.Select(x => x.Name).ToList();
        await userManager.AddToRolesAsync(adminUser, adminModeratorRoles);
    }
}
