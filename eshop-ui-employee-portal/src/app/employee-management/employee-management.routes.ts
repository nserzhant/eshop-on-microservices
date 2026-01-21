import { Routes } from '@angular/router';
import { authGuard } from '../auth/auth.guard';
import { EmployeesListComponent } from './employees-list/employees-list.component';
import { EmployeeEditComponent } from './employee-edit/employee-edit.component';

export const employeeManagementRoutes: Routes = [
  {
    path: 'employees',
    component: EmployeesListComponent,
    canActivate: [authGuard],
    data: { roles: ['Administrator'] }
  },
  {
    path: 'employees/:employeeId',
    component: EmployeeEditComponent,
    canActivate: [authGuard],
    data: { roles: ['Administrator'] }
  }
];
