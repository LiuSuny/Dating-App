using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
   [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

          //url of the photos 
        public string Url {get; set;}

        //this check if this their main photo or not
        public bool IsMain {get; set;}

         //this help with photo storage
        public string PublicId {get; set;}

         //relationship
        public AppUser AppUser {get; set;} //defining the relationship

        //acting a foreign key
        public int AppUserId {get; set;} //defining the relationship

    }
}