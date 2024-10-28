using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController :BaseApiController
    {
        public readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context;           
        }

      //use to test our 401 authorize responses
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
              return "secret text";
        }

        //404 request not found
         [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(){
             var thing = _context.Users.Find(-1);
            if(thing == null) return NotFound();

            return Ok(thing);
        }
         
         //500 server error
         [HttpGet("server-error")]
        public ActionResult<string> GetServerError(){
            
                var server = _context.Users.Find(-1);

                var serverToReturn = server.ToString();

                return serverToReturn; 
          
             
        }
           
           //bad request 400
         [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest(){
               
           // return BadRequest("this was not good request");
            return BadRequest();
        }
    }
}