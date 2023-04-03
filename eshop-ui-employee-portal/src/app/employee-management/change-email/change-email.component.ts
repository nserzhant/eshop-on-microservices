import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from '../../auth/authentication.service';
import { ApiErrorDTO, EmployeeAccountClient, EmployeeDTO } from '../services/api/employee.api.client';
@Component({
  selector: 'app-change-email',
  templateUrl: './change-email.component.html'
})
export class ChangeEmailComponent implements OnInit {

  isLoading = false;
  changeEmailForm!: FormGroup;
  apiError : ApiErrorDTO | null = null;

  constructor(
    private employeeAccountClient: EmployeeAccountClient,
    private authenticationService: AuthenticationService) {
  }

  ngOnInit(): void {
    this.changeEmailForm = new FormGroup({
      email: new FormControl('',[Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required])
    });
  }

  onSubmit() {
    if (!this.changeEmailForm.valid) {
      return;
    }

    const email = this.changeEmailForm.value.email;
    const password = this.changeEmailForm.value.password;
    const employeeDTO = new EmployeeDTO({email : email, password: password});

    this.isLoading = true;
    return this.employeeAccountClient.changeEmail(employeeDTO).subscribe({
      error: (e) => {
        this.isLoading = false;
        this.apiError = e;
      },
      complete: () => {
        this.isLoading = false;
        this.authenticationService.logout();
      }
    });
  }
}
