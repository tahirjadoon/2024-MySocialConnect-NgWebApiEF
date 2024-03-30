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
}
