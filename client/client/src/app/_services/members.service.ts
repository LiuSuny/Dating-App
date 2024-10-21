import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Output } from '@angular/core';
import { environement } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, Observable, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';



@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environement.apiUrl;
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient) { }

  getMembers(page?: number, itemPerPage?: number){
    let params = new HttpParams();
     //double checking we get a page
    if(page !==null && itemPerPage !== null){
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemPerPage.toString());
    }

    return this.http.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params})
    .pipe( map(response => {
      this.paginatedResult.result = response.body;
      if(response.headers.get('Pagination') !==null){
        this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return this.paginatedResult;
    })

    )


    //use before with caching
    //if(this.members.length > 0)return of(this.members);
      // return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      //   map(members => {
      //     this.members = members
      //     return members;
      //   })
      // )
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
}

