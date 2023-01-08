import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiErrorDTO } from 'src/app/services/api/users.api.client';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { passwordMatchConfirmPasswordValidator } from '../validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  isLoading = false;
  registerForm!: FormGroup;
  apiError : ApiErrorDTO | null = null;

  constructor(        
    private router: Router,
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
    this.authenticationService.register(email, password).subscribe({
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
