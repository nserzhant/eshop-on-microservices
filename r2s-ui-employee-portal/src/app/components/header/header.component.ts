import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { Roles } from 'src/app/services/api/users.api.client';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {
  isAuthenticated = false;
  isAdmin = false;
  userEmail = '';
  componentDestroyed$ = new Subject<void>();

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService) { }

  ngOnInit(): void {
    this.authenticationService.user.pipe(takeUntil(this.componentDestroyed$))
      .subscribe((user) => {        
        if (user) {
          this.isAuthenticated = true;
          this.userEmail = user.email!;
          if (user.roles!.indexOf(Roles.Administrator) >= 0) {
            this.isAdmin = true;
          } else {
            this.isAdmin = false;
          }
        } else {
          this.isAdmin = this.isAuthenticated = false;
          this.userEmail = '';
        }
      })
  }
  
  ngOnDestroy(): void {
    this.componentDestroyed$.next();
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/']);
  }
}
