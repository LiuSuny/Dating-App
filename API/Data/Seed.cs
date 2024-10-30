using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
        {
            if(await userManager.Users.AnyAsync())return; //checking if there is any data

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options); //deserialize our userData into an object
            if(users == null) return;

            //initialize a role
            var roles = new List<AppRole>
            {
                new AppRole {Name = "Member"},
                new AppRole {Name = "Admin"},
                new AppRole {Name = "Moderator"}

            };

            //create a role
            foreach(var role in roles)
            {
                 await roleManager.CreateAsync(role);
            }

            //Once it users is in list form we go ahead to add it to our db using foreach
            foreach(var user in users)
            {
                #region We now user identityUser
                //using var hmac = new HMACSHA512(); //HMACSHA512()
                 #endregion
                  
                user.UserName = user.UserName.ToLower();
                
                   #region We now user identityUser
                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                // user.PasswordSalt = hmac.Key;
                    #endregion

                //await  context.Users.Add(user);
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }
            //create for admin and moderatory
            var admin = new AppUser{
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
        }
    }
}