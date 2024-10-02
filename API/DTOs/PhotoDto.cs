using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotoDto
    {
         public int Id { get; set; }

          //url of the photos 
        public string Url {get; set;}

        //this check if this their main photo or not
        public bool IsMain {get; set;}
    }
}