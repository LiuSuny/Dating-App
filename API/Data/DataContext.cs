﻿using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>,
         AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        //creating our db table we r overriding this table b/c IdentityDbContext provide us with one
        //public DbSet<AppUser> Users { get; set; }

        public DbSet<AppUserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        //default ctor
        public DataContext(DbContextOptions options) : base(options) { }
        

        //configure the entity
       protected override void OnModelCreating(ModelBuilder builder)
       {
              base.OnModelCreating(builder);
            
            //many to one reletaionship
            builder.Entity<AppUser>()
                   .HasMany(ur => ur.UserRoles)
                   .WithOne(ur => ur.User)
                   .HasForeignKey(ur => ur.UserId)
                   .IsRequired();

           //many to one reletaionship
             builder.Entity<AppRole>()
                   .HasMany(ur => ur.UserRoles)
                   .WithOne(ur => ur.Role)
                   .HasForeignKey(ur => ur.RoleId)
                   .IsRequired();

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
              
              //first part of the relationship to our AppUser table
            builder.Entity<Message>()
                   .HasOne(u => u.Recipient)
                   .WithMany(m => m.MessagesReceived)
                   .OnDelete(DeleteBehavior.Restrict); // prevent user from deleting if other user has not read the message
           
           //second part of the relationship to our AppUser table
         builder.Entity<Message>()
                   .HasOne(u => u.Sender)
                   .WithMany(m => m.MessagesSent)
                   .OnDelete(DeleteBehavior.Restrict); // prevent user from deleting if other user has not read the message
         
       }
    }
}
