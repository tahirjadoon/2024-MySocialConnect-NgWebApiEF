using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Entities;

namespace MSC.Core.DB.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
}
