import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

/* Guard - provide a powerful way to implement security and control navigation flow 
in Angular applications, as it ensure that routes are accessed only by users 
who meet certain criteria in our case role of admin our application*/
export const adminGuard: CanActivateFn = ():Observable<boolean> => {
  
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  
   return accountService.currentUser$.pipe(
      map(user =>{
        if(user.roles.includes('Admin') || user.roles.includes('Moderator'))
         {
          return true;
         }
        toastr.error('Access Denied!');       
        return false
      })
    ) 
};
