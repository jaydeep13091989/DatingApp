import { JsonPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModelComponent } from 'src/app/models/roles-model/roles-model.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
users: User[] = [];
bsModelRef: BsModalRef<RolesModelComponent> = new BsModalRef<RolesModelComponent>();
availableRoles = [
  'Admin',
  'Moderator',
  'Member'
]
  constructor(private adminService: AdminService, private modelService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.adminService.getUsersWithRoles().subscribe({
      next: users => this.users = users
    })
  }
  openRolesModel(user: User)
  {
    const config = {
      class: 'model-dialog-centered',
      initialState: {
        username: user.userName,
        availableRoles: this.availableRoles,
        selectedRoles : [...user.roles]
      }
    }
    this.bsModelRef = this.modelService.show(RolesModelComponent, config);
    this.bsModelRef.onHide?.subscribe({
      next: () => {
          const selectedRoles = this.bsModelRef.content?.selectedRoles;
          if(!this.arrayEqual(selectedRoles!, user.roles))
          {
            this.adminService.updateUserRoles(user.userName, String(selectedRoles)).subscribe({
              next : roles => user.roles = roles
            })
          }
      }
    })
  }

  private arrayEqual(arr1: any[], arr2: any[])
  {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort());
  }
}
