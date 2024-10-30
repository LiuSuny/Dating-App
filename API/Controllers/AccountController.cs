using API.Entities;
using API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;           
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
         public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
         {
            //checking if the username already exist
            if(await UserExists(registerDTO.Username)) 
             return BadRequest("Username already taken");
            
            //map it
            var user = _mapper.Map<AppUser>(registerDTO);
             
             #region 
             //using var hmac = new HMACSHA512();
             #endregion
                //assign appuser to registerdto
               user.UserName = registerDTO.Username;

               #region 
              //  user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
              //  user.PasswordSalt = hmac.Key;
              #endregion

            //  _context.Users.Add(user);
            //  await _context.SaveChangesAsync();

            //creating user and password
             var result = await _userManager.CreateAsync(user, registerDTO.Password);
              
              //next we check if it doesnt succeded
              if(!result.Succeeded) return BadRequest(result.Errors);             
              
              var roleResult = await _userManager.AddToRoleAsync(user, "Member");
              //next we check if it doesnt succeded
              if(!roleResult.Succeeded) return BadRequest(result.Errors);    

             return new UserDTO{
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
             };

         }

        [HttpPost("login")]
         public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
         {
              //getting the user                 
              var user = await _userManager.Users
               .Include(x => x.Photos)
              .SingleOrDefaultAsync (x => x.UserName == loginDTO.Username.ToLower());
              
              //checking if user name is null then return Unauthorized
              if(user ==null) return Unauthorized("Invalid username");

               #region we are now using identityuser
            //  using var hmac = new HMACSHA512(user.PasswordSalt);
              
            //   var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        
            //     for(int i =0; i<ComputeHash.Length; i++)
            //     {

            //         if(ComputeHash[i] != user.PasswordHash[i]){
            //             return Unauthorized("Invalid password");
            //         }
            //     }           
              #endregion
             
              //however if username is find and succeeded then 
              var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
              
               if(!result.Succeeded) return Unauthorized();  

              return new UserDTO{
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
             };
         }

           private async Task<bool> UserExists(string username)
           {
                   return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
           }
    
    }
}