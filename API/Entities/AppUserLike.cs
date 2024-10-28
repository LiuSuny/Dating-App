using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    //class responsible for user likes etc
    public class AppUserLike
    {
        public AppUser SourceUser {get; set;}
        public int SourceUserId {get; set;}
        public AppUser TargetUser {get; set;}
        public int TargetUserId {get; set;}
    }
}