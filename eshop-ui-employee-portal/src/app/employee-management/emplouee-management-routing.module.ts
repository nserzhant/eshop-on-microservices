import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "../auth/auth-guard.service";
import { EmloyeesListComponent } from "./emloyees-list/emloyees-list.component";
import { EmployeeEditComponent } from "./employee-edit/employee-edit.component";

const routes: Routes = [
  {
    path: 'employee-management',
    children : [
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
