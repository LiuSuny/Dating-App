using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignaIR
{
    public class PresenceTracker
    {                                  //key- username  --connectionId-string
        private static readonly Dictionary<string, List<string>> OnlineUsers
        = new Dictionary<string, List<string>>(); 

         //method to add users to the dictionary when they connect
         public Task<bool> UserConnected(string username, string connectionId)
         {
            bool isOnline = false;

            //since dictionary is not thread safe we have to use thread to lock the users when they r connected

            lock(OnlineUsers)
            {
                //checking if the key contain username
                if(OnlineUsers.ContainsKey(username))
                {
                    //if it is the key of username then we add username list
                    OnlineUsers[username].Add(connectionId);
                }
                //if we don't have such key username then we create new one
                else
                {
                    OnlineUsers.Add(username, new List<string>{connectionId});
                     isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
         }
         
         //user diconnected
        public Task<bool> UserDisConnected(string username, string connectionId)
        {
            bool isOffline = false;

            lock(OnlineUsers)
            {
                //if there is no user with online presence then we complete our task
                 if(!OnlineUsers.ContainsKey(username))  return Task.FromResult(isOffline);
                 //then we remove
                 OnlineUsers[username].Remove(connectionId);

                 if(OnlineUsers[username].Count == 0)
                 {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                 }
            }
             return Task.FromResult(isOffline);
        }

        //Method to get all online users
          public Task<string[]> GetOnlineUsers()
          {
                 //create a variables for this
                 string[] onlineUsers;

                 lock(OnlineUsers)
                 {
                    //Adding our dictionary to an array and select all user 
                    //Note: that in this case we r using memory storage not our db
                     onlineUsers =  OnlineUsers            
                    //Sorts the elements of a sequence in ascending order according to a key(username)
                     .OrderBy(k => k.Key)
                    //Projects each element of a sequence into a new form.
                     .Select(k => k.Key)
                     .ToArray();//An array that contains the elements from the input sequence.
                 }
                 return Task.FromResult(onlineUsers); // The successfully completed task.
          }

            public Task<List<string>> GetConnectionForUser(string username)
            {
                List<string> connectionId;
                
                lock(OnlineUsers)
                {
                    /*GetValueOrDefault(username) Tries to get the value associated with the specified key in 
                    the dictionary.
                    Returns:A List instance. When the method is successful, 
                    the returned object is the value associated with the specified key.
                     When the method fails, it returns the default value for List.*/

                    connectionId = OnlineUsers.GetValueOrDefault(username);                              
                }
                  return Task.FromResult(connectionId);
            }
    }
}