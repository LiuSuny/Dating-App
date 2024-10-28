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
    public class MessageRepository : IMessageRepository
    {
       
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message); 
        }

        public void DeleteMessage(Message message)
        {
          _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
          return await _context.Messages.FindAsync(id);

          //return await _context.Messages.FirstOrDefaultAsync(m => m.Id ==id);
           
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            //getting messages sent and order then be recent sent
            var query = _context.Messages
                .OrderByDescending(m => m.DateMessageSent).AsQueryable();

            //checking message container and which one to return
            query = messageParams.Container switch 
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username),
                //way to default container
                _ =>query.Where(u => u.Recipient.UserName 
                == messageParams.Username && u.DateRead ==null) //which means message might not be read yet
                
            };

            //Next we need to project to and return dto
            var message = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider );
            return await PagedList<MessageDto>.CreateAsync(message, messageParams.PageNumber, messageParams.PageSize);

        }

        //this message implement the message of both side of the conversation
        //And we mark any unread message read
        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, 
        string recipientUsername)
        {
            //"Include" works well with list of object(eager loading), but if we need to get multi-level data, 
            //in the case then "ThenInclude" is the best fit as we need to get access to our user photos 
            //which is in another third tables 
            //GETTING CONVERSATION OF THE USERS 
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
               .Where(m => m.Recipient.UserName == currentUsername
                && m.Sender.UserName == recipientUsername
                || m.Recipient.UserName ==recipientUsername
                && m.Sender.UserName == currentUsername
               )
               .OrderBy(m => m.DateMessageSent)
               .ToListAsync();

               //check if there is unread message 
               var unReadMessage = messages.Where
                (m => m.DateRead ==null && m.Recipient.UserName == currentUsername).ToList();
                 
                 //we mark then as read
                if(unReadMessage.Any()){
                  //loop
                  foreach(var message in unReadMessage)
                  {
                    message.DateRead = DateTime.Now;
                  }
                  //saved it
                  await _context.SaveChangesAsync();
                }
                 
                 //we map then into dto
                return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
 
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; //this allow us to return boolean
        }
    }
}