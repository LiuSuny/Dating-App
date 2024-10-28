using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    //setting up number of pages
    public class UserParam : PaginationParams
    {
        
        public string CurrentUsername { get; set; }
        public string Gender { get; set; }

        public int MinAge {get; set;} = 18;
        public int MaxAge {get; set;} = 150;
        //add sort
        public string OrderBy {get; set;} = "lastActive";

    }
}