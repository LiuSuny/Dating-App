import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environement } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environement.apiUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRole(){
    return this.http.get<Partial<User[]>>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRole(username: string, roles: string[]){
    //https://localhost:5000/api/admin/edit-roles/lisa?roles=Moderator,Member
    return this.http.post(this.baseUrl + 'admin/edit-roles/' +username + '?roles=' + roles, {});
  }
}
