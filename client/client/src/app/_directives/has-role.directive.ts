import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';
import { User } from '../_models/user';

/*Directives are defined as classes that can add new 
behavior to the elements in the template or modify existing behavior. 
The purpose of Directives in Angular is to maneuver the DOM,
 be it by adding new elements to DOM or removing elements and
even changing the appearance of the DOM elements
  list of directive we have use so far are 
  *ngIf, 
  *ngFor,
   bsRadio
  however we be using *AppHasRole directive in this case
  */

@Directive({
  selector: '[appHasRole]'
})

export class HasRoleDirective implements OnInit{
/*Reminder that @Input decorator is used to pass data from a parent component to a child component.*/
  @Input() appHasRole: string[];
  user: User;
  
  constructor(private viewContainerRef: ViewContainerRef, 
    private templateRef: TemplateRef<any>, private accountService: AccountService
  )
   {
       this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
           this.user = user;
       })
   }
  ngOnInit(): void {
     //clear the view if no role
     if(!this.user?.roles || this.user == null){
      this.viewContainerRef.clear();
      return;
     }
     //some - Determines whether the specified callback function returns true for any element of an array.
     if(this.user?.roles.some(r => this.appHasRole.includes(r))){
       this.viewContainerRef.createEmbeddedView(this.templateRef);
     }else{
      this.viewContainerRef.clear();
     }
  }

}
