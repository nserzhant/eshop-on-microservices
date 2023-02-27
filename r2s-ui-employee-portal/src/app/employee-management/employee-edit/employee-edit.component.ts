import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { Location } from '@angular/common';
import { ApiErrorDTO, EmployeeManagementClient, EmployeeReadModel, Roles } from '../services/api/employee.api.client';

@Component({
  selector: 'app-employee-edit',
  templateUrl: './employee-edit.component.html'
})
export class EmployeeEditComponent implements OnInit {

  employee?: EmployeeReadModel;
  roles?: String[];
  isRolesUpdating  = false;
  isPasswordUpdating = false;
  updateRolesApiError : ApiErrorDTO | null = null;
  updatePasswordApiError : ApiErrorDTO | null = null;

  constructor(
    private employeeManagementClient: EmployeeManagementClient,
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
    const user$ = this.employeeManagementClient.get(employeeId);
    this.employee = await lastValueFrom(user$);
    this.roles = this.employee.roles!.map(r=>r.name!);
  }

  changeRoles() {
    const roles = this.roles?.map(r=> Roles[r as keyof typeof Roles]) || new Array<Roles>();

    this.isRolesUpdating = true;
    this.employeeManagementClient.setRoles(this.employee?.id!, roles).subscribe({
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
    this.employeeManagementClient.setPassword(this.employee?.id!,password).subscribe({
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
