using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Entities;
using MSC.Core.Extensions;

namespace MSC.Core.DB.Data;

public class SeedData
{
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
            user.PasswordHash = hashkey.Hash;
            user.PasswordSalt = hashkey.Salt;
            user.UserName = user.UserName.ToLowerInvariant();

            context.Users.Add(user);
        }

        //save to database 
        await context.SaveChangesAsync();

    }
}
