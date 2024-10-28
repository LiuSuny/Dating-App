using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
      
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
           
        }

        public async Task<AppUserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async  Task<PagedList<LikesDto>> GetUserLikes(LikesParams likesParams)
        {
            var users =  _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes =  _context.Likes.AsQueryable();
             
             //if the object is equal to liked
            if(likesParams.Predicate == "liked"){
                //then we filter and identify what is going on
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser); // this give us users from our liked table and passing it to users table

            }
            
             if(likesParams.Predicate == "likedBy"){
                //then we filter and identify what is going on
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser); 
            }

          var likeUsers = users.Select(user => new LikesDto{
                
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id

            });

            return await PagedList<LikesDto>
            .CreateAsync(likeUsers, likesParams.PageNumber, likesParams.PageSize);
        }

         //getting user with collection of likes with Id
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                      .Include(u => u.LikedUsers)
                      .FirstOrDefaultAsync(x => x.Id == userId);
                      
        }
    }
}