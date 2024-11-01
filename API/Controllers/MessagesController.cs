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
        private readonly IUnitOfWork _unitOfWork;
         private readonly IMapper _mapper;
        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

       [HttpPost]
       public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto){
        
          //first we try user details
          var username  = User.GetUsername();
          
          //check if username is the same as recipient then return badrequest
           if(username == createMessageDto.RecipientUsername.ToLower()) 
           return BadRequest("You cannot send messages to yourself");

           //getting hold of the message sender
           var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            //getting hold of the message recipient from our createdmessagedto
           var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

           if(recipient == null) return NotFound();

           //next we create new message
           var message = new Message {
              Sender = sender,
              Recipient = recipient,
              SenderUserName = sender.UserName,
              RecipientUserName = recipient.UserName,
              Content = createMessageDto.Content
           };

             _unitOfWork.MessageRepository.AddMessage(message);
             if(await _unitOfWork.Complete()) 
             return Ok(_mapper.Map<MessageDto>(message));

             return BadRequest("Failed to send message");
       }

       
       [HttpGet]
       public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser
       ([FromQuery]MessageParams messageParams){
             
             //add username to messageparams
             messageParams.Username = User.GetUsername();
             //get user message 
             var message = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

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
            return Ok( await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
       }
        
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            //get hold of the user
            var username = User.GetUsername();

            //get hold of the message
            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            //if the message sender and recipient is not equal to the username then they are not authorized to delete the message
             if(message.Sender.UserName != username
              && message.Recipient.UserName != username)
              return Unauthorized();

            //if the send username and receiver the same then senddelete is true 
            if(message.Sender.UserName == username) message.SendDelete = true;
            if(message.Recipient.UserName == username) message.RecipientDelete = true;

           //if both the sender and recipient deleted the message then we remove it from the server
            if(message.SendDelete && message.RecipientDelete)
            _unitOfWork.MessageRepository.DeleteMessage(message);

           if(await _unitOfWork.Complete()) return Ok();

           return BadRequest("Problem deleting the message");
        }




    } 
}