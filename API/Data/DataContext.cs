using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        //creating our db table
        public DbSet<AppUser> Users { get; set; }

        //default ctor
        public DataContext(DbContextOptions options) : base(options) { }
        
    }
}
