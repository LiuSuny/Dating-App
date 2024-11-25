import { ActivatedRouteSnapshot, Resolve, ResolveFn, RouterStateSnapshot } from "@angular/router";
import { Member } from "../_models/member";
import { Observable } from "rxjs";
import { MembersService } from "../_services/members.service";
import { inject, Injectable } from "@angular/core";

/* Angular Resolver is used to pre-fetch data when the user navigates
  from one route to another. It can be defined as a smooth approach 
 to enhancing user experience by loading data before
  the user navigates to a particular component.*/

  @Injectable({
    providedIn: 'root'
  })

export class MemberDetailedResolver implements Resolve<Member>{

    constructor(private  memberService: MembersService){}
    resolve(route: ActivatedRouteSnapshot):
     Observable<Member>  {
      
        return this.memberService.getMember(route.paramMap.get('username'));
    }
    
}

// export const memberDetailedResolver: ResolveFn<Member | null> 
//= (route, state) => {
//     const memberService = inject(MembersService);
  
//     const username = route.paramMap.get('username');
  
//     if (!username) return null;
  
//     return memberService.getMember(username);
//   };