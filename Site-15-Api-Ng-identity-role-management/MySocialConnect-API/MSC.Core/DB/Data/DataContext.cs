using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Entities;

namespace MSC.Core.DB.Data;

/*
don't foget to add migrations
dotnet ef migrations add MessageEntityAdded -o Core/DB/Migrations (first time)
dotnet ef migrations add MessageEntityAdded (after wards)
and then either issue command "dotnet ef database update"
or do dontnet run. For this check program.cs "CUSTOM: Seed Data Start" section
*/

//IR_REFATCOR
//public class DataContext : DbContext
public class DataContext : IdentityDbContext<AppUser, //class we created
                                            AppRole, //class we created
                                            int, //type int as that is what we defined AppUser, AppRole
                                            IdentityUserClaim<int>, 
                                            AppUserRole, //class we created
                                            IdentityUserLogin<int>,
                                            IdentityRoleClaim<int>, 
                                            IdentityUserToken<int>
                                            >
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    //IR_REFATCOR
    /*
    //Photos will be pulled with the user so no need to put here. Check Photo entity for details
    public DbSet<AppUser> Users { get; set; } 
    */

    //table name will be created as "Likes"
    //creating the db set so that we can directly query
    public DbSet<UserLike> Likes { get; set; }

    //table name will be created as "Messages"
    //creating the db set so that we can directly query
    public DbSet<UserMessage> Messages {get; set;}

    //override OnModelCreating to create the relationshops for the likes
    //give entities some configuration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //IR_REFATCOR Due to use of Identity
        CreateUserRole(builder);

        UserLikeSetup(builder);
        UserMessageSetup(builder);
    }

    //IR_REFATCOR Many to Many relationship
    private void CreateUserRole(ModelBuilder builder)
    {
        builder.Entity<AppUser>()
                    .HasMany(ur => ur.UserRoles)
                    .WithOne(u => u.User) //Property in AppUserRole
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired()
            ;

        builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role) //Property in AppUserRole
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
        ;
    }

    //user like configuration
    private void UserLikeSetup(ModelBuilder builder)
    {
        //key is combination of SourceUserId and TargetUserId
        builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.TargetUserId});

        //build relationships between AppUser and UserLike. Here the users liked by the logged in user
        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser) //UserLike field
            .WithMany(l => l.LikedUsers) //AppUser field
            .HasForeignKey(s => s.SourceUserId) 
            //for sql server the same entity cannot have to cascades so one needs to DeleteBehavior.NoAction 
            .OnDelete(DeleteBehavior.Cascade) //when the user is deleted then delete the related entities. 
        ;

        //build relationships between AppUser and UserLike. Here the logged in user liked by other users
        builder.Entity<UserLike>()
            .HasOne(t => t.TargetUser) //UserLike field
            .WithMany(l => l.LikedByUsers) //AppUser field
            .HasForeignKey(t => t.TargetUserId) 
            //for sql server the same entity cannot have to cascades so one needs to DeleteBehavior.NoAction 
            .OnDelete(DeleteBehavior.Cascade) //when the user is deleted then delete the related entities
        ;
    }

    private void UserMessageSetup(ModelBuilder builder)
    {
        //primary key is by convention so no need to create
        builder.Entity<UserMessage>()
            .HasIndex(t => t.Guid);

        //receiver
        //for sql server the same entity cannot have to cascades so one needs to DeleteBehavior.NoAction 
        builder.Entity<UserMessage>()
            .HasOne(r => r.Recipient)
            .WithMany(m => m.MessagesReceived)
            .HasForeignKey(t => t.RecipientId)
            .OnDelete(DeleteBehavior.Cascade)
        ;

        //sender
        //for sql server the same entity cannot have to cascades so one needs to DeleteBehavior.NoAction 
        builder.Entity<UserMessage>()
            .HasOne(s => s.Sender)
            .WithMany(m => m.MessagesSent)
            .HasForeignKey(s => s.SenderId)
            .OnDelete(DeleteBehavior.Cascade)
        ;
    }
}
