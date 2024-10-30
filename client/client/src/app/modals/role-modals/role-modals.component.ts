import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-role-modals',
  templateUrl: './role-modals.component.html',
  styleUrls: ['./role-modals.component.css']
})
export class RoleModalsComponent implements OnInit{
  /*EventEmitter is a class that facilitates communication 
  between components, primarily from child to parent components.
   It's a way for a child component to emit custom events that
    the parent component can listen to and respond to.*/

  @Input() updateSelectedRoles = new EventEmitter();

  user: User;
  roles: any[];

  // title: string;
  // list: any[] =[];
  // closeBtnName: string;
  
  constructor(public bsModalRef: BsModalRef){}

  ngOnInit(): void {
    
  }

  updateRole(){
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}
