import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginCallbackComponent } from './auth/login-callback.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login-callback', component: LoginCallbackComponent },
  {
    path: 'catalog',
    loadChildren: () => import('./catalog/catalog.routes').then(m => m.catalogRoutes)
  },
  {
    path: 'employee-management',
    loadChildren: () => import('./employee-management/employee-management.routes').then(m => m.employeeManagementRoutes)
  }
];
