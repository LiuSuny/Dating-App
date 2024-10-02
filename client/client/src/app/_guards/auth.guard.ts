import { inject } from '@angular/core';
import { CanActivate, CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map, Observable, tap } from 'rxjs';

 
export const AuthGuard: CanActivateFn = ():Observable<boolean> => {

  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  
   return accountService.currentUser$.pipe(
      map(user =>{
        if(user)
          return true;
        toastr.error('Access Denied!');       
        return false
      })
    ) 
  
  
};


// export class authGuard implements CanActivate {
  
//   constructor(private accountService: AccountService, private toastr: 
//        ToastrService) { }

//        canActivate(): Observable<boolean> {
//       return this.accountService.currentUser$.pipe(
//        map(user => {
//         if (user)
//           return true;                          
//          this.toastr.error('Access Denied');
//             return false    
//          //map(user => !!user),
//          //tap(user => user || this.toastr.error('Access Denied') )         
//       })
//     )
//   }
// }