import { Component, Input, OnInit } from '@angular/core';
import { ApiErrorDTO, IdentityErrorsDTO, UsersDomainErrorDTO } from 'src/app/services/api/users.api.client';

@Component({
  selector: 'api-errors-summary',
  templateUrl: './api-errors-summary.component.html',
  styleUrls: ['./api-errors-summary.component.css']
})
export class ApiErrorsSummaryComponent implements OnInit {

  identityErrors: IdentityErrorsDTO | null = null;
  userDomainError: UsersDomainErrorDTO | null = null;

  @Input()
  set apiError(apiError : ApiErrorDTO | null) {
    this.identityErrors = null;
    this.userDomainError = null;
    
    if (apiError != null) {
      if (apiError.identityErrors != null) {
        this.identityErrors = apiError.identityErrors;
      }

      if (apiError.domainError != null) {
        this.userDomainError = apiError.domainError;
      }
    }
  }
  
  constructor() { }

  ngOnInit(): void {
  }
}
