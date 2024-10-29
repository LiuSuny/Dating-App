import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environement } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environement.apiUrl;

  constructor(private http: HttpClient) { }
  
  //to get all messages
  getMessages(pageNumber, pageSize, container){
    let params = getPaginationHeader(pageNumber, pageSize);
     params = params.append('Container', container);

     return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string){
      return this.http.get<Message[]>(this.baseUrl + 'messages/thread/'+ username)
  }

}