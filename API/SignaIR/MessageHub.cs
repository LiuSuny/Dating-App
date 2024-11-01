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
        private readonly IUnitOfWork _unitOfWork;      
        private readonly  IHubContext<PresenceHub> _presenceHub;
         private readonly PresenceTracker _tracker;
        public MessageHub(IUnitOfWork unitOfWork,
        IMapper mappper, 
         IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            _tracker = tracker;
            _mappper = mappper;
            _unitOfWork = unitOfWork;
            _presenceHub = presenceHub;
        } 

          //overriding the hub virtual method
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

             var group = await AddToGroup(groupName);
             
             await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
             

            //if a user join a group
            var message = await _unitOfWork.MessageRepository
            .GetMessageThread(Context.User.GetUsername(),
            otherUser);
            
            if(_unitOfWork.HasChanges()) await _unitOfWork.Complete();
            
            //make user send message
            await Clients.Caller.SendAsync("ReceiveMessageThread", message);
        }

          //overriding the hub virtual method to disconnect user
         public override async Task OnDisconnectedAsync(Exception exception)
        {
           var group = await RemoveFromMessageGroup();

            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

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
           var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            //getting hold of the message recipient from our createdmessagedto
           var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

           if(recipient == null) throw new HubException("User not found");

           //next we create new message
           var message = new Message {
              Sender = sender,
              Recipient = recipient,
              SenderUserName = sender.UserName,
              RecipientUserName = recipient.UserName,
              Content = createMessageDto.Content
           };
             var groupName = GetGroupName(sender.UserName, recipient.UserName);
             
             var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

             if(group.connections.Any(x => x.Username == recipient.UserName))
             {
                  message.DateRead = DateTime.UtcNow;
             }
             else{
                var connections = await _tracker.GetConnectionForUser(recipient.UserName);
                if(connections != null){
                    await _presenceHub.Clients.Clients(connections)
                    .SendAsync("NewMessageReceived", new{username = sender.UserName, 
                    knownAs = sender.KnownAs});
                }
             }

             _unitOfWork.MessageRepository.AddMessage(message);

             if(await _unitOfWork.Complete())
             {
                 await Clients.Group(groupName).SendAsync("NewMessage", _mappper.Map<MessageDto>(message));
             } 

         }
        
       
       /// <summary>
       /// HubCallerContext is  abstract class that allow us access to User, claim and connectionId etc
       /// </summary>
       /// <param name="context"></param>
       /// <param name="groupName"></param>
       /// <returns></returns>
        private async Task<Group> AddToGroup(string groupName)
        {
            //getting name
             var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName); 
             //attaching id to new user connections
             var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
              
              //check if the new connect is null
              if(group == null)
              {
                   group = new Group(groupName);
                   _unitOfWork.MessageRepository.AddGroup(group);

              }

              group.connections.Add(connection);

              if(await _unitOfWork.Complete()) 
              return group;
            throw new HubException("Unable to join to group");
        }
         
         //remove from message group
         private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _unitOfWork.MessageRepository
            .GetGroupForConnection(Context.ConnectionId);

            var connection = group.connections
            .FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            _unitOfWork.MessageRepository.RemoveConnection(connection);
            if(await _unitOfWork.Complete()) return group;

            throw new HubException("Failed to remove from group");

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