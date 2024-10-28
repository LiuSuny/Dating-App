import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit{
  
  /*Partial type is a utility type in TypeScript that allows user to
   create a new type by making all properties of an existing type OPTIONAL. 
   This is useful when working with APIs in our case that return objects with a lot of properties, 
   but not all of them are REQUIRED sort of like iqueryable in case of c#*/

  members: Partial<Member[]>;

  predicate = 'liked';

  constructor(private memberService: MembersService) {}


  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
     this.memberService.getLikes(this.predicate).subscribe(response => {
       this.members = response
     })
  }

}
