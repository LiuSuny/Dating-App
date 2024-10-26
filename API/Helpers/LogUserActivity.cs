using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    /// <summary>
    /// IAsyncActionFilter :A filter that asynchronously surrounds execution of the action,
    /// after model binding is complete.
    /// </summary>
        public class LogUserActivity : IAsyncActionFilter
    {
        //implement method from IAsyncActionFilter
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //getting hold of the context we get from the next
            var resultContext = await next();

            //checking if users is authenticated
             if (context.HttpContext.User.Identity?.IsAuthenticated != true) return; //return nothing id they r not
             
             //if they authenticated then 
             var id = resultContext.HttpContext.User.GetUserId();
             //var username = resultContext.HttpContext.User.GetUsername();


             var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
             
            
             var userId = await repo.GetUserByIdAsync(id);
             //var user = await repo.GetUserByUsernameAsync(username);
             
              userId.LastActive = DateTime.Now;
              //user.LastActive = DateTime.Now;
              await repo.SaveAllAsync();
             

       

       


        }
    }
}