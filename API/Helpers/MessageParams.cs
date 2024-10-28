using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class MessageParams : PaginationParams
    {
        //current login username
        public string Username  { get; set; }
        //container that contained all message if the message is unread or read
        public string Container { get; set; } = "unread"; //defualt is unread
        
    }
}