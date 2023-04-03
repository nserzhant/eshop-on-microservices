import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "../auth/auth-guard.service";
import { ChangeEmailComponent } from "./change-email/change-email.component";
import { ChangePasswordComponent } from "./change-password/change-password.component";
import { EmloyeesListComponent } from "./emloyees-list/emloyees-list.component";
import { EmployeeEditComponent } from "./employee-edit/employee-edit.component";

const routes: Routes = [
  {
    path: 'employee-management',
    children : [
      {
        path: 'change-email',
        component: ChangeEmailComponent,
        canActivate : [AuthGuard]
      },
      {
        path: 'change-password',
        component: ChangePasswordComponent,
        canActivate : [AuthGuard]
      },
      {
        path: 'employees',
        component: EmloyeesListComponent,
        canActivate : [AuthGuard],
        data: {roles: ['Administrator']}
      },
      {
        path: 'employees/:employeeId',
        component: EmployeeEditComponent,
        canActivate : [AuthGuard],
        data: {roles: ['Administrator']}
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class EmployeeManagementRoutingModule {}
