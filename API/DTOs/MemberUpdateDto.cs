using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class MemberUpdateDto
    {
        //Introduction  
        public string Introduction { get; set; }

        //looking for who
        public string LookingFor {get; set;}

        //Interested in who
        public string Interests { get; set; }

        //city
        public string City {get; set;}

        //country
        public string Country {get; set;}

    }
}