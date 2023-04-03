import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from './authentication.service';

@Component({
  selector: 'login-callback',
  template: `<p>{{ 'process-login-callback' | translate }}</p>`
})
export class LoginCallbackComponent implements OnInit {

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService) {

    this.authenticationService.userManager.signinCallback().then(()=> {
        this.router.navigate(['']);
        this.authenticationService.handleUserLoggedIn();
      }
    );
   }

  ngOnInit(): void {
  }
}
