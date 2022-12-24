import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { ApiErrorDTO, Roles, UserReadModel, UsersClient } from 'src/app/services/api/users.api.client';
import { Location } from '@angular/common';

@Component({
  selector: 'app-employee-edit',
  templateUrl: './employee-edit.component.html',
  styleUrls: ['./employee-edit.component.css']
})
export class EmployeeEditComponent implements OnInit {

  user?: UserReadModel;
  roles?: String[];
  isRolesUpdating  = false;
  isPasswordUpdating = false;
  updateRolesApiError : ApiErrorDTO | null = null;
  updatePasswordApiError : ApiErrorDTO | null = null;

  constructor(    
    private usersClient: UsersClient,
    private route: ActivatedRoute,
    private location: Location) { 
   }

  ngOnInit(): void {
      this.route.params.subscribe((params: Params) => {
        const id = params['employeeId'];
        this.loadUser(id);
      });
  }

  async loadUser(employeeId: string) {
    const user$ = this.usersClient.get(employeeId);
    this.user = await lastValueFrom(user$);
    this.roles = this.user.roles!.map(r=>r.name!);
  }

  changeRoles() {
    const roles = this.roles?.map(r=> Roles[r as keyof typeof Roles]) || new Array<Roles>();
    
    this.isRolesUpdating = true;
    this.usersClient.saveRoles(this.user?.id!, roles).subscribe({
          error: (e) => {
          this.isRolesUpdating = false;
          this.updateRolesApiError = e; 
          },
          complete: () => { 
            this.isRolesUpdating = false;
            this.updateRolesApiError = null; 
          }
      });
  }

  changePassword(form: NgForm) {
    if (!form.valid) {
      return;
    }
    
    const password = form.value.password;
    
    this.isPasswordUpdating = true;
    this.usersClient.setPassword(this.user?.id!,password).subscribe({
          error: (e) => {
          this.isPasswordUpdating = false;
          this.updatePasswordApiError = e; 
          },
          complete: () => { 
            this.isPasswordUpdating = false;
            this.updatePasswordApiError = null; 
          }
      });
  }

  back() {
    this.location.back();
  }
}
