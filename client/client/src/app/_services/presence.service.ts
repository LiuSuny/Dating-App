import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environement } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

 hubUrl = environement.hubUrl;

 //private property we get from signaLR
 private hubConnection: HubConnection;

 //BehaviorSubject is a generic observable variant of Subject that requires an initial
 //  value and emits its current value whenever it is subscribed to so changes in sort of 
 //whenever users comes online it show and offline also show 
 private onlineUserSource = new BehaviorSubject<string[]>([]);

 onlineUsers$ =this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService) { }

  //method that let the user connect to our presence hub once user is authenticated
  //we are using User- b/c we need to send in our jwt token or user token when we make this connection
  //as we can not pass in our http request in this case like the interceptor as this going to be different
  //And we be using WEBSOCKET which has no support from authentication headers
  createHubConnection(user: User)
  {
      //this take care of creating the hub connection
     this.hubConnection = new HubConnectionBuilder()
         .withUrl(this.hubUrl + 'presence',  //passing in the url
          {
             accessTokenFactory: () => user.token
         })
         .withAutomaticReconnect() //if there is a network problem our user is going to try and  reconnect to our hub
         .build();

         //next we start the hubconnection
      this.hubConnection
          .start()
          .catch(error => console.log(error));

      //listen to server event if user is online 
      this.hubConnection.on('UserIsOnline', username => {
        this.toastr.info(username + ' has connected')
      })
          
      //listen to server event if user is offline
      this.hubConnection.on('UserIsOffOnline', username => {
        this.toastr.warning(username + ' has disconnected')
      })
      
      //geting the current users online
      this.hubConnection.on('GetOnlineUsers', (username: string[]) => {
        this.onlineUserSource.next(username);
      })
  }
  

  stopHubConnection()
  {   
         //next we stop the hubconnection
      this.hubConnection
          .stop()
          .catch(error => console.log(error));    
  }

}
