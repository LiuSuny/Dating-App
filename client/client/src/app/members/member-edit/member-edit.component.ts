import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { map, Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit{
  @ViewChild('editForm') editForm: NgForm; //this give us access to editform inside component

  //Hostliesterner monitor several event in this we want to monitor window browser event
  //This notify users when user want to close window tabs when editing profile
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event:any){
    if(this.editForm.dirty){
      $event.returnValue = true
    }
  }

  member: Member;
  user: User;

  constructor(private accountService: AccountService,
     private memberService: MembersService,
    private toastr: ToastrService){

      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
     }

  ngOnInit(): void {
    this.loadMember();
  }
    
  loadMember(){
   this.memberService.getMember(this.user.username)
    .subscribe((member: any) => {
      this.member = member
     
    }) 
  }
 
  
  

  updateMember(){
    this.memberService.updateMemeber(this.member).subscribe(() =>{
      this.toastr.success('Profile updated successfully');
      this.editForm.reset(this.member);
    })
   
  }
}
