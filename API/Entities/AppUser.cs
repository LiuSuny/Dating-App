

using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        //User Id
        public int Id { get; set; }

        //User name
        public string UserName { get; set; }
        
        //for hashing our password
        public byte[] PasswordHash {get; set;}

        //for salting password
        public byte[] PasswordSalt {get; set;}
         
        public DateOnly DateOfBirth {get; set;}

         //know as name might be different like nickname
        public string KnownAs {get; set;}

        //date created
        public DateTime Created {get; set;} = DateTime.Now;

         //Last time active
        public DateTime LastActive {get; set;} = DateTime.Now;

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
        public List<Photo> Photos { get; set; } = new();//one to many relationship in ef
       
        //list of liked the current login users have gotten 
        public List<AppUserLike> LikedByUsers { get; set; } = new();

        //list of current users that login user has liked 
        public List<AppUserLike> LikedUsers { get; set; } = new();
        public List<Message> MessagesSent { get; set; } = new();
        public List<Message> MessagesReceived { get; set; } = new();



        //public int GetAge()
        //{
        //    return DateOfBirth.CalculateAge();
        //}
    }
   
}
