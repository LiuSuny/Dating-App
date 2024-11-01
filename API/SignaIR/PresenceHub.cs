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
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        //overriding the hub virtual method
        public override async Task OnConnectedAsync()
        {
            //tracking user with username and id --connectionId from Hub
           var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if(isOnline)
            //Clients, context and sendasync are all part of hub properties we implement method when user is online
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            //Getting list of all current online user
            var currentUsers = await _tracker.GetOnlineUsers();
             await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
            
        }
        
            //overriding the hub virtual method
         public override async Task OnDisconnectedAsync(Exception exception)
        {

            //tracking user with username and id --connectionId from Hub
           var isOffline = await _tracker.UserDisConnected(Context.User.GetUsername(), Context.ConnectionId);
              if(isOffline)
            //Clients, context and sendasync are all part of hub properties we implement method when user is offline
            await Clients.Others.SendAsync("UserIsOffOnline", Context.User.GetUsername());
            
            // //Getting list of all current online user
            // var currentUsers = await _tracker.GetOnlineUsers();
            //  await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
            
            //if there is exception we pass it on to the base class to handle
            await base.OnDisconnectedAsync(exception);
        }

    }
}