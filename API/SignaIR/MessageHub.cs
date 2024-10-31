using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignaIR
{
    //Class responsible for user message connect
    public class MessageHub : Hub
    {
        private readonly IMapper _mappper;       
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        public MessageHub(IMessageRepository messageRepository,
        IMapper mappper, IUserRepository userRepository)
        {
            _mappper = mappper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

          //overriding the hub virtual method
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //if a user join a group
            var message = await _messageRepository.GetMessageThread(Context.User.GetUsername(),
            otherUser);
            //make user send message
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", message);
        }

          //overriding the hub virtual method to disconnect user
         public override async Task OnDisconnectedAsync(Exception exception)
        {
           await base.OnDisconnectedAsync(exception);
        }
        
        /// <summary>
        /// method responsible for sending live messages to other users
        /// </summary>
        /// <param name="createMessageDto"></param>
        /// <returns></returns>
         public  async Task SendMessage(CreateMessageDto createMessageDto)
         {
        
                 //first we try user details
           var username  = Context.User.GetUsername();
          
          //check if username is the same as recipient then return badrequest
           if(username == createMessageDto.RecipientUsername.ToLower()) 
           throw new HubException("You can not send message to yourself");

           //getting hold of the message sender
           var sender = await _userRepository.GetUserByUsernameAsync(username);
            //getting hold of the message recipient from our createdmessagedto
           var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

           if(recipient == null) throw new HubException("User not found");

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
             {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                 await Clients.Group(group).SendAsync("NewMessage", _mappper.Map<MessageDto>(message));
             } 

         }

         /// <summary>
         ///Compare string method
         /// </summary>
         /// <param name="caller"></param>
         /// <param name="other"></param>
         /// <returns></returns>
        private string GetGroupName(string caller,  string other)
        {
               var stringCompare = string.CompareOrdinal(caller, other) < 0;
               return stringCompare ? $"{caller} -{other}" : $"{other}-{caller}";
        }


    }
}