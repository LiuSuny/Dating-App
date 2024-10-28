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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public MessagesController(IMessageRepository messageRepository,
         IMapper mapper, IUserRepository userRepository)
        {
            _userRepository = userRepository;          
            _mapper = mapper;
            _messageRepository = messageRepository;
            
        }

       [HttpPost]
       public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto){
        
          //first we try user details
          var username  = User.GetUsername();
          
          //check if username is the same as recipient then return badrequest
           if(username == createMessageDto.RecipientUsername.ToLower()) 
           return BadRequest("You cannot send messages to yourself");

           //getting hold of the message sender
           var sender = await _userRepository.GetUserByUsernameAsync(username);
            //getting hold of the message recipient from our createdmessagedto
           var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

           if(recipient == null) return NotFound();

           //next we create new message
           var message = new Message {
              Sender = sender,
              Recipient = recipient,
              SenderUserName = sender.UserName,
              RecipientUserName = recipient.UserName,
              Content = createMessageDto.Content
           };

             _messageRepository.AddMessage(message);
             if(await _messageRepository.SaveAllAsync()) 
             return Ok(_mapper.Map<MessageDto>(message));

             return BadRequest("Failed to send message");
       }

       
       [HttpGet]
       public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser
       ([FromQuery]MessageParams messageParams){
             
             //add username to messageparams
             messageParams.Username = User.GetUsername();
             //get user message 
             var message = await _messageRepository.GetMessagesForUser(messageParams);

             //get our pagination
             Response.AddPaginationHeader(message.CurrentPage, message.PageSize,
              message.TotalCount, message.TotalPages);

             return Ok(message);

       }

        [HttpGet("thread/{username}")] //geting hold our other paticipant username is since we can get hold of current user inside ur controller
       public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread
       (string username)
       {
            //geting hold of current username
            var currentUsername = User.GetUsername();
            //other paticipant username
            return Ok( await _messageRepository.GetMessageThread(currentUsername, username));
       }
        
    } 
}