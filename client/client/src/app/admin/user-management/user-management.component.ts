import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RoleModalsComponent } from 'src/app/modals/role-modals/role-modals.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit{

  users: Partial<User[]>;
  bsModalRef: BsModalRef

  constructor(private adminService: AdminService, 
    private modalService: BsModalService){}


  ngOnInit(): void {
   this.getUsersWithRole();
  }

  getUsersWithRole(){
    this.adminService.getUsersWithRole().subscribe(data => {
      this.users = data;
    })
  }

  openRoleModal(user: User){
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    }
   
    this.bsModalRef = this.modalService.show(RoleModalsComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe(value => {
      const rolesToUpdate = {
        roles: [...value.filter(el => el.checked ===true).map(el=>el.name)]
      };
      if(rolesToUpdate){
        this.adminService.updateUserRole(user.username, rolesToUpdate.roles).subscribe(() => {
          user.roles = [...rolesToUpdate.roles]
        })
      }
    })
  }

  private getRolesArray(user){
   const roles = [];
   const userRoles = user.roles;
   const availibleRoles: any[] = [
    {name: 'Admin', value: 'Admin'},
    {name: 'Moderator', value: 'Moderator'},
    {name: 'Member', value: 'Member'}  
  ]
  availibleRoles.forEach(role => {
    let isMatch = false;
    for(const userRole of userRoles){
      if(role.name === userRole){
        isMatch = true;
        role.checked = true;
        roles.push(role);
        break;
      }
    }
    if(!isMatch){
      role.checked=false;
       roles.push(role);
    }
  })
   return roles;
  }
}
