import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Output } from '@angular/core';
import { environement } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, Observable, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';



@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environement.apiUrl;
  members: Member[] = [];
  

  constructor(private http: HttpClient) { }

  getMembers(userParams: UserParams){
   
    let params = this.getPaginationHeader(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);

    return this.getPaginatedResult<Member[]>(this.baseUrl +'users', params);
  }
  
 

  getMember(username:string){
     const member = this.members.find(x => x.username ===username);
     if(member !== undefined) of(member)
    return this.http.get<Member[]>(this.baseUrl + 'users/' + username);
  
  }

  updateMemeber(member:Member){
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/'+ photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/'+ photoId);
  }


  private getPaginatedResult<T>(url, params){

    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
     return this.http.get<T>(url, {observe: 'response', params})
     .pipe( map(response => {
       paginatedResult.result = response.body;
       if(response.headers.get('Pagination') !==null){
         paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
       }
       return paginatedResult;
     })
 
     )
   }
   
   private getPaginationHeader(pageNumber: number, pageSize: number){
     
     let params = new HttpParams();
     //double checking we get a page
    //if(page !==null && itemPerPage !== null){
      params = params.append('pageNumber', pageNumber.toString());
      params = params.append('pageSize', pageSize.toString());
    
      return params;
   }
}

