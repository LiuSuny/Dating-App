using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class MemberDto
    {
          //User Id
        public int Id { get; set; }

        //User name
        public string Username { get; set; }
        
        //photo url
        public string PhotoUrl {get; set;} 
         //checking age
        public int Age {get; set;}

         //know as name might be different like nickname
        public string KnownAs {get; set;}

        //date created
        public DateTime Created {get; set;} 

         //Last time active
        public DateTime LastActive {get; set;} 

         //Gender
        public string Gender {get; set;}

        //Introduction 
        public string Introduction {get; set;}

         //looking for who
        public string LookingFor {get; set;}
      
        //Interested in who
        public string Interests {get; set;} 

        //city
        public string City {get; set;}

        //country
        public string Country {get; set;}

         //Collection of class photo
        public ICollection<PhotoDto> Photos {get; set;} 

    }
}