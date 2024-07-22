import { Component, Input, OnInit } from '@angular/core';
import { ApiErrorDTO, IdentityErrorsDTO } from 'src/app/employee-management/services/api/employee.api.client';

@Component({
  selector: 'api-errors-summary',
  templateUrl: './api-errors-summary.component.html'
})
export class ApiErrorsSummaryComponent implements OnInit {

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

  constructor() { }

  ngOnInit(): void {
  }
}
