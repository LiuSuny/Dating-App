import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { RouterOutlet } from '@angular/router';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  //imports: [RouterOutlet, NavComponent, HomeComponent, NgxSpinnerComponent]
})
export class AppComponent implements OnInit {

  title = 'The Dating App';
  users:any;

  constructor(private accountService: AccountService, 
    private presenceService: PresenceService){}

   ngOnInit() {
     this.setCurrentUser();
   }
   
   setCurrentUser(){
       const user :User = JSON.parse(localStorage.getItem("user"));
      if(user) {
        this.accountService.setCurrentUser(user);
        this.presenceService.createHubConnection(user);
      }

      // const userString = localStorage.getItem('user');
      // if (!userString) return;
      // const user = JSON.parse(userString);
      // this.accountService.setCurrentUser(user);
   }

  
}
