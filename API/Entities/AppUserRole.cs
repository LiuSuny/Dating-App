using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    /// <summary>
    /// Responsible for user role and responsible for
    /// join entities with many to many relationship
    /// </summary>
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User {get; set;}
        public AppRole Role {get; set;} 

    }
}