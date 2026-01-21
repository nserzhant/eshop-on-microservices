import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { CommonModule, Location } from '@angular/common';
import { ApiErrorDTO, EmployeeManagementClient, EmployeeReadModel, Roles } from '../services/api/employee.api.client';
import { TranslateModule } from '@ngx-translate/core';
import { ApiErrorsSummaryComponent } from '../api-errors-summary/api-errors-summary.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MATERIAL_COMPONENTS_IMPORTS } from 'src/app/shared/material-imports';

@Component({
    selector: 'app-employee-edit',
    templateUrl: './employee-edit.component.html',
    imports: [
      CommonModule,
      FormsModule,
      TranslateModule,
      ApiErrorsSummaryComponent,
      ...MATERIAL_COMPONENTS_IMPORTS
    ]
})
export class EmployeeEditComponent implements OnInit {
  private employeeManagementClient = inject(EmployeeManagementClient);
  private route = inject(ActivatedRoute);
  private location = inject(Location);

  employee = signal<EmployeeReadModel | undefined>(undefined);
  roles = signal<string[]>([]);
  isRolesUpdating = signal(false);
  isPasswordUpdating = signal(false);
  updateRolesApiError = signal<ApiErrorDTO | null>(null);
  updatePasswordApiError = signal<ApiErrorDTO | null>(null);
  destroyRef$ = inject(DestroyRef);

  ngOnInit(): void {
    this.route.params.pipe(takeUntilDestroyed(this.destroyRef$)).subscribe((params: Params) => {
      const id = params['employeeId'];
      this.loadUser(id);
    });
  }

  async loadUser(employeeId: string) {
    const user$ = this.employeeManagementClient.get(employeeId);
    const employeeData = await lastValueFrom(user$);
    this.employee.set(employeeData);
    this.roles.set(employeeData.roles!.map(r=>r.name!));
  }

  async changeRoles() {
    const rolesArray = this.roles()?.map(r=> Roles[r as keyof typeof Roles]) || new Array<Roles>();

    this.isRolesUpdating.set(true);
    try {
      await lastValueFrom(this.employeeManagementClient.setRoles(this.employee()?.id!, rolesArray));
      this.updateRolesApiError.set(null);
      // Reload user to get updated roles from server
      await this.loadUser(this.employee()?.id!);
    } catch (e) {
      this.updateRolesApiError.set(e as ApiErrorDTO);
    } finally {
      this.isRolesUpdating.set(false);
    }
  }

  async changePassword(form: NgForm) {
    if (!form.valid) {
      return;
    }

    const password = form.value.password;

    this.isPasswordUpdating.set(true);
    try {
      await lastValueFrom(this.employeeManagementClient.setPassword(this.employee()?.id!, password));
      this.updatePasswordApiError.set(null);
    } catch (e) {
      this.updatePasswordApiError.set(e as ApiErrorDTO);
    } finally {
      this.isPasswordUpdating.set(false);
    }
  }

  back() {
    this.location.back();
  }
}
