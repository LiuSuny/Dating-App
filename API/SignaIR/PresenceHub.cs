using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignaIR
{
    [Authorize]
    //Hub- is an abstract base class fro signaIR
    public class PresenceHub : Hub
    {
        //overriding the hub virtual method
        public override async Task OnConnectedAsync()
        {
            //Clients, context and sendasync are all part of hub properties we implement method when user is online
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            
        }
        
            //overriding the hub virtual method
         public override async Task OnDisconnectedAsync(Exception exception)
        {
            //Clients, context and sendasync are all part of hub properties we implement method when user is offline
            await Clients.Others.SendAsync("UserIsOffOnline", Context.User.GetUsername());
            
            //if there is exception we pass it on to the base class to handle
            await base.OnDisconnectedAsync(exception);
        }
    }
}