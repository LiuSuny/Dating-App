import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
import { ReplaySubject } from 'rxjs';
import { environement } from 'src/environments/environment';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseURL= environement.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  login(model: any) {
    return this.http.post(this.baseURL + 'account/login', model)
    .pipe(
      map((response:User) => {
        const user = response;
        if(user){
          //localStorage.setItem('user', JSON.stringify(user));
          //this.currentUserSource.next(user);
          this.setCurrentUser(user);
          this.presenceService.createHubConnection(user);
        }
      })
    )
  }
  
  register(model: any) {
    return this.http.post(this.baseURL + 'account/register', model)
    .pipe(
      map((user: User) => {
        if(user){
          //localStorage.setItem('user', JSON.stringify(user));
          //this.currentUserSource.next(user);
          this.setCurrentUser(user);
          this.presenceService.createHubConnection(user);
        }       
      })
    )
  }

  setCurrentUser(user:User){
    user.roles = [];
    const roles = this.getDecordedToken(user.token).role;
      //checking if the user is inform or array or not 
      Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

     localStorage.setItem('user', JSON.stringify(user));
     this.currentUserSource.next(user);
  }
  
  logout(){
    
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();
    //this.currentUserSource = new ReplaySubject<User>();    
  }
  
  getDecordedToken(token){
    //Atob() stands for ASCII to Binary. It a string of data which has been
    // encoded using Base64  to convert binary data like images, videos etc
    //Note:  Tokens - consist of three parts separated by dots ( . ), 
    //which are: Header. Payload. Signature. only the Signature are encrypted
     //and the part we r interested in is the payload which the middle hence [1] after split('.')
      return JSON.parse(atob(token.split('.')[1]));
  }
}
