using System.ComponentModel;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
       
        public static int GetUserId(this ClaimsPrincipal user)
        {
            ////this give us user by id from the token base authentication
          var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? throw new Exception("Cannot get username from token"));
        
          return userId;
        }

         //this method find user by name or uniquename 
        public static string GetUsername(this ClaimsPrincipal user)
        {
            //    //User: built In User claim in controllerbase and FindFirst() Retrieves the first claim with the specified claim type. Returns:
            //   //The first matching claim or null if no match is found.
             var username = user.FindFirstValue(ClaimTypes.Name) 
            ?? throw new Exception("Cannot get username from token");
        
            return username;
        }

    }
}
