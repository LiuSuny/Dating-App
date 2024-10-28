using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        //creating our db table
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppUserLike> Likes { get; set; }

        //default ctor
        public DataContext(DbContextOptions options) : base(options) { }
        

        //configure the entity
       protected override void OnModelCreating(ModelBuilder builder)
       {
              base.OnModelCreating(builder);
            
            //configure our primary since we did not specify inside our AppUserLike entity
            //and it going be combination of sourceuserid and likeuserid
            builder.Entity<AppUserLike>().HasKey(k => new {
                k.SourceUserId, k.TargetUserId
            });
             
            //configure the relationship so one user can like many other users
            builder.Entity<AppUserLike>()
                   .HasOne(s => s.SourceUser) //current login user 
                    .WithMany(l => l.LikedUsers) //liking many others
                    .HasForeignKey(s => s.SourceUserId) //specifying the foreign key                  
                   //once we delete user then we delete users entity including the likes
                   //Note: in the case of SQL serve this need to DeleteBehavior.NoAction
                   .OnDelete(DeleteBehavior.Cascade); 

                //configure the relationship so one user can like many other users
            builder.Entity<AppUserLike>()
                   .HasOne(x => x.TargetUser) //current login user 
                    .WithMany(l => l.LikedByUsers) //liking many others
                   .HasForeignKey(s => s.TargetUserId) //specifying the foreign key
                   .OnDelete(DeleteBehavior.Cascade); //delete the user then we delete entity including the likes

         
       }
    }
}
