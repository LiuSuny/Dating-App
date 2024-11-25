import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environement } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, pipe, take } from 'rxjs';
import { Route, Router } from '@angular/router';

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

  constructor(private toastr: ToastrService, private router: Router) { }

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
       this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUserSource.next([...usernames, username])
       })
      })
          
      //listen to server event if user is offline
      this.hubConnection.on('UserIsOffOnline', username => {
        this.onlineUsers$.pipe(take(1)).subscribe(usernames =>{
          this.onlineUserSource.next([...usernames.filter(x => x !==username)])    
        })   
         })
      
      //geting the current users online
      this.hubConnection.on('GetOnlineUsers', (username: string[]) => {
        this.onlineUserSource.next(username);
      })
        
       //notfy users that they get a message
      this.hubConnection.on('NewMessageReceived', ({username, knownAs}) => {
        this.toastr.info(knownAs + ' has sent you new message!')
        //next we need to route this message to where we want on user page since we using toastr
         .onTap
         .pipe(take(1))
         .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'));
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
