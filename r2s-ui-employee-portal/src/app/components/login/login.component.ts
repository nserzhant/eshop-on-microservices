import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiErrorDTO } from 'src/app/services/api/users.api.client';
import { AuthenticationService } from 'src/app/services/authentication.service';

/* Implicit login Flow. Currently is not using. */
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  isLoading = false;
  loginForm!: FormGroup;
  apiError : ApiErrorDTO | null = null;

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService) {
     }

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      email: new FormControl('',[Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required])
    });
  }

  onSubmit() {
    // if (!this.loginForm.valid) {
    //   return;
    // }

    // const email = this.loginForm.value.email;
    // const password = this.loginForm.value.password;    
    // this.isLoading = true; 
    
    // this.authenticationService.login(email, password).subscribe({
    //     error: (e) => {
    //       this.isLoading = false;
    //       this.apiError = e; 
    //     },
    //     complete: () => {
    //       this.isLoading = false;
    //       this.router.navigate(['/']);
    //     }
    // });
  }
}