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
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if(await userManager.Users.AnyAsync())return; //checking if there is any data

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options); //deserialize our userData into an object
            
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
            }
             //Note: the UserManager take care of saving the password to do db automatically without adding the savechanges
             //await context.SaveChangesAsync(); 
        }
    }
}