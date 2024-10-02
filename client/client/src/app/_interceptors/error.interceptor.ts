import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    return next.handle(request).pipe(
      catchError(error =>{
        //error check 
         if(error){
          switch (error.status) {
            //returning 400 check 
            case 400:
              if(error.error.errors){
                const modelStateError =[];
                for(const key in error.error.errors){
                   if(error.error.errors[key]){
                     modelStateError.push(error.error.errors[key]) //the idea is to flatten the--- 
                     //error response we get back from our validation response and push then into array modelStateError =[];
                   }
                }
                throw modelStateError.flat();
              }else if(typeof(error.error) === 'object'){
                this.toastr.error(error.statusText === "OK" ? "Bad Request" : error.statusText, error.status); //added this line because statusText defualt to Ok instead of real statusCode Online helper
                 //this.toastr.error(error.statusText, error.status);
              }else{
                 this.toastr.error(error.error, error.status);
              }
              break;
              case 401:
                 //this.toastr.error(error.error === null ? "Unauthorized" : error.error, error.status)
                this.toastr.error(error.statusText === "OK" ? "Unauthorised" : error.statusText, error.status);
                //this.toastr.error(error.statusText, error.status);
                break;
                case 404:
                this.router.navigateByUrl('/not-found');
                break;
                case 500:
                  const navigationExtra: NavigationExtras = {state:{error: error.error}}
                  this.router.navigateByUrl('/server-error', navigationExtra);
                  break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
         }
         return throwError(error); //deprecated
         //return throwError(() => new Error(error))
      })
      )
  }
}
