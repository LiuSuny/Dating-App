using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }
        
       [HttpPost("{username}")]
       public async Task<ActionResult> AddLike(string username){
            
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);

            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound(); //if user you intend liking is null return notfound
            //prevent users from liking themself
            if(sourceUser.UserName == username) return BadRequest("You cannot like yourself"); //You can't liked yourself
             
             //double checking if appuserlike has already like the current user
             var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
              //return badrequest if they already like the current user
              if(userLike !=null) return BadRequest("You already like this user");
             
             //Next we mapped it
             userLike = new AppUserLike
             {
                  SourceUserId = sourceUserId,
                  TargetUserId =  likedUser.Id
             };

             //add map
             sourceUser.LikedUsers.Add(userLike);

             //next we save our changes
             if(await _userRepository.SaveAllAsync()) return Ok();

             return BadRequest("Failed to like user");
       }

        //Get user likes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikesDto>>> GetUserLikes([FromQuery]LikesParams likesParams){
          
           likesParams.UserId = User.GetUserId();
          var user = await _likesRepository.GetUserLikes(likesParams);

           Response.AddPaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount, user.TotalPages);
           
           return Ok(user);

       }
            
       
    }
}