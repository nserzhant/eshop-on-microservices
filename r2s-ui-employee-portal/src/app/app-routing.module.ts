import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChangeEmailComponent } from './components/change-email/change-email.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { EmloyeesListComponent } from './components/emloyees-list/emloyees-list.component';
import { EmployeeEditComponent } from './components/employee-edit/employee-edit.component';
import { HomeComponent } from './components/home/home.component';
import { LoginCallbackComponent } from './components/login-callback/login-callback.component';
import { RegisterComponent } from './components/register/register.component';
import { AuthGuard } from './services/auth-guard.service';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: 'login-callback',
    component: LoginCallbackComponent
  },
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
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
