import { Component, Input } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ApiErrorDTO, IdentityErrorsDTO } from 'src/app/employee-management/services/api/employee.api.client';

@Component({
    selector: 'api-errors-summary',
    templateUrl: './api-errors-summary.component.html',
    imports: [
      MatFormFieldModule
    ]
})
export class ApiErrorsSummaryComponent {
  identityErrors: IdentityErrorsDTO | null = null;

  @Input()
  set apiError(apiError : ApiErrorDTO | null) {
    this.identityErrors = null;

    if (apiError != null) {
      if (apiError.identityErrors != null) {
        this.identityErrors = apiError.identityErrors;
      }
    }
  }
}
