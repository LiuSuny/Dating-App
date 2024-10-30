using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {
            //configuring users with roles
            var users = await _userManager.Users //get hold of user from appuser
                       .Include(r => r.UserRoles) //include a userroles from AppUserRole
                       .ThenInclude(r => r.Role) //include a role  from AppRole
                       .OrderBy(u => u.UserName) //sort by username from appuser
                       .Select(u => new           //next we select id, userName roles
                        {
                          u.Id,
                          Username = u.UserName,
                          Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                        })
                       .ToListAsync();   //send to list

             return Ok(users); //then return 
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            //query coming up in comma sperated list
            var selectedRoles = roles.Split(",").ToArray();
             
             //get user
            var user  = await _userManager.FindByNameAsync(username);
              
              if(user == null) return NotFound("Could not find user");
            //get user role
            var userRoles = await _userManager.GetRolesAsync(user);

            //take a look at the user role and check whether they r there on role or need to be added
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            //check if adding succeeded
            if(!result.Succeeded) return BadRequest("Failed to add to roles");

            //removing
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            //check if remove from role succeeded
            if(!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));

        }


        [Authorize(Policy = "moderatePhotoRole")]
        [HttpGet("photo-to-moderate")]
        public ActionResult GetPhotoforModeration()
        {
             return Ok("Admin  or Moderator can see this");
        }
    }
}