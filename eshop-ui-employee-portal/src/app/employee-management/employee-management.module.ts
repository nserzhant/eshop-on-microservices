import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ApiErrorsSummaryComponent } from './api-errors-summary/api-errors-summary.component';
import { EmployeeEditComponent } from './employee-edit/employee-edit.component';
import { EmloyeesListComponent } from './emloyees-list/emloyees-list.component';
import { MaterialModule } from 'src/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EmployeeManagementClient, EMPLOYEE_API_URL } from './services/api/employee.api.client';
import { environment } from 'src/environments/environment';
import { EmployeeManagementRoutingModule } from './emplouee-management-routing.module';



@NgModule({
  declarations: [
    ApiErrorsSummaryComponent,
    EmployeeEditComponent,
    EmloyeesListComponent
  ],
  imports: [
    CommonModule,
    TranslateModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    EmployeeManagementRoutingModule
  ],
  providers: [
    {
      provide: EMPLOYEE_API_URL,
      useValue: environment.apiRoot
    },
    EmployeeManagementClient
  ]
})
export class EmployeeManagementModule { }
