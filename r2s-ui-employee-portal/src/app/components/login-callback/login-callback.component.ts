import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'login-callback',
  templateUrl: './login-callback.component.html',
  styleUrls: ['./login-callback.component.css']
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
