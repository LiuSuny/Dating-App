using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            //    //User: builtIn User claim in controllerbase and FindFirst() Retrieves the first claim with the specified claim type. Returns:
            //   //The first matching claim or null if no match is found.
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value; //this give us user username from the token base authentication

        }
    }
}
