using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace API.Controllers
{
    
    public class UsersController : BaseApiController
    {
       
       private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository,  IMapper mapper,
            IPhotoService photoService) //remember to return
        {           
            _photoService = photoService;
            _userRepository = userRepository;
            _mapper = mapper;          
        }

      
         [HttpGet]      
         public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {           
             var users = await _userRepository.GetMembersAsync();            
             return Ok(users);
            
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task <ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberByNameAsync(username);
        }

        
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
             
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

             if (user == null) return NotFound();
           
           
            _mapper.Map(memberUpdateDto, user);
                
           // _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync())
            
            return NoContent();
                
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddUserPhoto(IFormFile file)
        {
           
            //get user by name
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            //result back from photoservice
            var result = await _photoService.AddPhotoAsync(file);

            //checking if upload resut to an error check
            if (result.Error != null)
                return BadRequest(result.Error.Message);

            //create new photo from ...
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            //checking if the photo is a main photo or not and we can go ahead to st it as main
            if (user.Photos.Count == 0)
                photo.IsMain = true;
            
            ////if it is then add
            user.Photos.Add(photo);

            //saving photos with help of mappers
            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtRoute(nameof(GetUser), new { username= user.UserName}, _mapper.Map<PhotoDto>(photo));       
                   
            }

            //ese if not loaded badrequest
            return BadRequest("Problem adding photo");


        }
    }
}
