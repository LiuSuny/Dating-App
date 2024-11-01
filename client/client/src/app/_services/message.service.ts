import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environement } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environement.apiUrl;
  hubUrl = environement.hubUrl;

  private hubConnection: HubConnection; 
  
  private messageThreadSource = new BehaviorSubject<Message[]>([]);

 messageThread$ =this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }
  

  createHubConnection(user: User, otherUserName: string)
  {
      //this take care of creating the hub connection
     this.hubConnection = new HubConnectionBuilder()
         .withUrl(this.hubUrl + 'message?user='+ otherUserName,  //passing in the url
          {
             accessTokenFactory: () => user.token
         })
         .withAutomaticReconnect() //if there is a network problem our user is going to try and  reconnect to our hub
         .build();

         //next we start the hubconnection
      this.hubConnection
          .start()
          .catch(error => console.log(error));

          // //listen to server event if user is online 
          this.hubConnection.on('ReceiveMessageThread', messages => {
             this.messageThreadSource.next(messages);
          })
              //send live messages
          this.hubConnection.on('NewMessage', message => {
            this.messageThread$.pipe(take(1)).subscribe(messages => {
              //To avoid mutating an array we use spread operator
              this.messageThreadSource.next([...messages, message])
            })
          })
          
          // //geting the current updated users connection 
          this.hubConnection.on('UpdatedGroup', (group: Group) => {
            if(group.connections.some(x => x.username == otherUserName)) 
            {
              this.messageThread$.pipe(take(1)).subscribe(messages => {
                messages.forEach(message => {
                  if(!message.dateRead){
                    message.dateRead = new Date(Date.now())
                  }
                })
                this.messageThreadSource.next([...messages]);
              })
            }
          })
  }
  
  stopHubConnection()
  {   
         //next we stop the hubconnection
     if(this.hubConnection)
     {
      this.hubConnection.stop();

     }
  }



  //to get all messages
  getMessages(pageNumber, pageSize, container){
    let params = getPaginationHeader(pageNumber, pageSize);
     params = params.append('Container', container);

     return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string){
      return this.http.get<Message[]>(this.baseUrl + 'messages/thread/'+ username)
  }
  
  //when we indicate a method with  async it gaurantee we return a promise
   async sendMessages(username: string, content: string){
   //return this.http.post<Message[]>(this.baseUrl +'messages', {recipientUserName: username, content})

   return this.hubConnection.invoke('SendMessage', {recipientUserName: username, content})
      .catch(error => console.log(error));
  }

  deleteMessage(id: number){
   return this.http.delete<Message[]>(this.baseUrl +'messages/'+id);

  }
}
