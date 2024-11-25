import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user : User;
  //add option to select if user is male or female
  genderList = [
                {value: 'male', display: 'Males'}, 
                {value: 'female', display: 'Females'}
               ]
 
  
  constructor(private memberService:MembersService) {
     this.userParams = this.memberService.getUserParam()
  }
  
  ngOnInit(): void {
    //this.members = this.memberService.getMembers();
    this.loadMembers();
  }
    
  loadMembers(){
    this.memberService.setUserParam(this.userParams);
    this.memberService.getMembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }
   
  resetFilters(){
    //reset to initial param
    this.userParams = this.memberService.resetUserParam();
     this.loadMembers();
  }
  pageChanged(event: any){
    this.userParams.pageNumber = event.page;
    this.memberService.setUserParam(this.userParams);
    this.loadMembers();
  }
}
