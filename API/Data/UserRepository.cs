using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            
        }

        public async Task<MemberDto> GetMemberByNameAsync(string username)
        {
            //This is the way to manually mapped our member if we are not using DTO
            // return await _context.Users
            // .Where(x => x.UserName == username)
            // .Select(user => new MemberDto{
            //     Id = user.Id,
            //     Username = user.UserName, 
            //     PhotoUrl = user.PhotoUrl,               
            // }).SingleOrDefaultAsync();

            //---------------------- but we IMapper we use ProjectTo
            return await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParam userParam)
        {
           var query = _context.Users.AsQueryable();
             
             // .AsQueryable();
              //filtering using username
              query = query.Where(u => u.UserName != userParam.CurrentUsername);
               //filtering using gender
              query = query.Where(u => u.Gender == userParam.Gender);

            //adding filtering age             
             var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParam.MaxAge - 1));
             var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParam.MinAge));

             query = query.Where(x => x.DateOfBirth > minDob && x.DateOfBirth < maxDob);
              
              //check lastive and sorting

              query = userParam.OrderBy switch 
              {
                 "created" => query.OrderByDescending(u => u.Created),
                 _ =>query.OrderByDescending(u => u.LastActive)
               
              };
              
              //.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
              //.AsNoTracking()
              //this same as the line above just that we r filtering before get to code line below
              return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(
               _mapper.ConfigurationProvider ).AsNoTracking(),            
              userParam.PageNumber, userParam.PageSize);
                    
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.
                Include(p => p.Photos).
                //SingleOrDefaultAsync(x => x.UserName == username);
                FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<string> GetUserGenderAsync(string username)
        {
           return await _context.Users
                 .Where(x => x.UserName == username)
                 .Select(x => x.Gender).FirstOrDefaultAsync();

                
                
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos) //eager loading 
            .ToListAsync();
             
        }

        // public async Task<bool> SaveAllAsync()
        // {
        //     return await _context.SaveChangesAsync() > 0; //saving all changes
        // }

        //this modified any updated entity
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}