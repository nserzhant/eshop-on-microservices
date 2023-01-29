import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ApiErrorDTO } from 'src/app/services/api/employee.api.client';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { passwordMatchConfirmPasswordValidator } from '../validators';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

  isLoading = false;
  changePasswordForm!: FormGroup;
  apiError : ApiErrorDTO | null = null;

  constructor(
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
    
    this.authenticationService.changePassword(currentPassword, password).subscribe({
        error: (e) => {
          this.isLoading = false;
          this.apiError = e; 
        },
        complete: () => {
          this.isLoading = false;
        } 
    });
  }
}
