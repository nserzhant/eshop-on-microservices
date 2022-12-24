import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiErrorDTO, IdentityErrorsDTO, UsersClient, UsersDomainErrorDTO } from 'src/app/services/api/users.api.client';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-change-email',
  templateUrl: './change-email.component.html',
  styleUrls: ['./change-email.component.css']
})
export class ChangeEmailComponent implements OnInit {

  isLoading = false;
  changeEmailForm!: FormGroup;
  apiError : ApiErrorDTO | null = null;

  constructor(
    private router: Router,
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
    this.isLoading = true; 
    
    this.authenticationService.changeEmail(email, password).subscribe({
        error: (e) => {
          this.isLoading = false;
          this.apiError = e; 
        },
        complete: () => {
          this.isLoading = false;
          this.router.navigate(['/login']);
        } 
    });
  }
}
