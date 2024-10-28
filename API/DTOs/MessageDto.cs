using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class MessageDto
    {
        //each meesage require Id which will be generated by db
        public int Id { get; set; }

        //we need track sender id
        public int SenderId { get; set; }
        //messager username 
        public string SenderUserName { get; set; }
        //relationship of property to sender
        public string SenderPhotoUrl {get; set;}
        //recipient id
        public int RecipientId { get; set; }
         //messager reciver username
        public string RecipientUserName { get; set; }
        //relationship of property to recipient
        public string RecipientPhotoUrl {get; set;}
        //message content
        public string Content { get; set; }
        //time message is read 
        public DateTime? DateRead {get; set;} 
        //time the message is send
        public DateTime DateMessageSent {get; set;} 
         

    }
}