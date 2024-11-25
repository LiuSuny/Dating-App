import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent  implements OnInit{
  
  registerMode = false;
  //users : any;
  
  constructor() {  
  }

  //this ctor is when we are passing component from parent to child component
  //constructor(private httpClient: HttpClient) {}
    
  
  
  ngOnInit(): void {
   //this.getUsers();
  }

  registerToggle(){
    this.registerMode = !this.registerMode;
  }

  // getUsers(){
  //   this.httpClient.get('https:///api/users').subscribe(users =>
  //     this.users = users

  //     //console.log('error response from api', response);
  //   );    
  //  }

   cancelRegisterMode(event:boolean){
      this.registerMode = event;
   }
}
