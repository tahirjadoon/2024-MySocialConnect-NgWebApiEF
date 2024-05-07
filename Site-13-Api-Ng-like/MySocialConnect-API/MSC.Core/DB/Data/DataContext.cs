using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Entities;

namespace MSC.Core.DB.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    //Photos will be pulled with the user so no need to put here. Check Photo entity for details
    public DbSet<AppUser> Users { get; set; } 

    //table name will be created as "Likes"
    //creating the db set so that we can directly query
    public DbSet<UserLike> Likes { get; set; }

    //override OnModelCreating to create the relationshops for the likes
    //give entities some configuration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        UserLikeSetup(builder);
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
}
