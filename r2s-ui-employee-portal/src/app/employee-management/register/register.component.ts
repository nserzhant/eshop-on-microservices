import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from '../../auth/authentication.service';
import { ApiErrorDTO, EmployeeAccountClient, EmployeeDTO } from '../services/api/employee.api.client';
import { passwordMatchConfirmPasswordValidator } from '../validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

  isLoading = false;
  registerForm!: FormGroup;
  apiError : ApiErrorDTO | null = null;

  constructor(
    private employeeAccountClient: EmployeeAccountClient,
    private authenticationService: AuthenticationService) {
  }

  ngOnInit(): void {
    this.registerForm = new FormGroup({
      email: new FormControl('',[Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl('', [Validators.required])
    }, [passwordMatchConfirmPasswordValidator()]);
  }

  onSubmit() {
    if (!this.registerForm.valid) {
      return;
    }

    const email = this.registerForm.value.email;
    const password = this.registerForm.value.password;
    const confirmPassword = this.registerForm.value.confirmPassword;

    if(password!== confirmPassword) {
      return;
    }

    this.isLoading = true;

    const employeeDTO = new EmployeeDTO({password: password, email: email});
    return this.employeeAccountClient.regiser(employeeDTO).subscribe({
        error: (e) => {
          this.isLoading = false;
          this.apiError = e;
        },
        complete: () => {
          this.isLoading = false;
          this.authenticationService.login();
        }
    });
  }
}
