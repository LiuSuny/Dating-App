import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Output } from '@angular/core';
import { environement } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, Observable, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';



@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environement.apiUrl;
  members: Member[] = [];
  //Note: When we want to store something with key and value we MAP-sort of like dictionary in c#
  memberCache = new Map();
  user : User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
   }

   getUserParam(){
    return this.userParams;
   }

   setUserParam(params: UserParams){
     this.userParams = params;
   }

   resetUserParam(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
   }

  getMembers(userParams: UserParams){
    // adding caching
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response){
      return of(response);
    }

    let params = getPaginationHeader(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    //return this.getPaginatedResult<Member[]>(this.baseUrl +'users', params);
    //Added caching
    return getPaginatedResult<Member[]>(this.baseUrl +'users', params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )

  }
  
 
  getMember(username:string){
   //using spread operator to get individual caching
   const member= [... this.memberCache.values()] //this can return numbers of array result 
   //to get single user in array we use Reduce
   .reduce((arr, elem) => arr.concat(elem.result), [])
   .find((member:Member) => member.username === username);
   if(member){
    return of(member);
   }
   return this.http.get<Member[]>(this.baseUrl + 'users/' + username);
 
 }

  // getMember(username:string){
  //    const member = this.members.find(x => x.username ===username);
  //    if(member !== undefined) of(member)
  //   return this.http.get<Member[]>(this.baseUrl + 'users/' + username);
  
  // }

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

  addLikes(username: string){
    //{url}likes/username
      return this.http.post(this.baseUrl + 'likes/'+ username, {});//we must add empty body since it is post
  }
 
  getLikes(predicate: string, pageNumber, pageSize){
    //adding pagination our likes
    let params = getPaginationHeader(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    //{url}likes?predicate=liked or likeBy (predicate represent)
    //return this.http.get<Partial<Member[]>>(this.baseUrl + 'likes?predicate='+ predicate);
    //passing the pagination instead
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params, this.http);
}

  
}

