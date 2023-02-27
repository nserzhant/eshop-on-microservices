import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from '../../auth/authentication.service';
import { ApiErrorDTO, ChangePasswordDTO, EmployeeAccountClient } from '../services/api/employee.api.client';
import { passwordMatchConfirmPasswordValidator } from '../validators';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html'
})
export class ChangePasswordComponent implements OnInit {

  isLoading = false;
  changePasswordForm!: FormGroup;
  apiError : ApiErrorDTO | null = null;

  constructor(
    private employeeAccountClient: EmployeeAccountClient,
    private authenticationService: AuthenticationService) {
  }

  ngOnInit(): void {
    this.changePasswordForm = new FormGroup({
      currentPassword: new FormControl('',[Validators.required]),
      password: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl('', [Validators.required])
    }, [passwordMatchConfirmPasswordValidator()]);
  }

  onSubmit() {
    if (!this.changePasswordForm.valid) {
      return;
    }

    const currentPassword = this.changePasswordForm.value.currentPassword;
    const password = this.changePasswordForm.value.password;
    const confirmPassword = this.changePasswordForm.value.confirmPassword;

    if( password !== confirmPassword ) {
      return;
    }

    this.isLoading = true;
    const changePasswordDTO = new ChangePasswordDTO({oldPassword : currentPassword, newPassword : password});

    this.employeeAccountClient.changePassword(changePasswordDTO).subscribe({
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
