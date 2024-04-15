import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Subject, take, takeUntil } from 'rxjs';
import { AuthenticationService } from '../auth/authentication.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit, OnDestroy {
  ADMINISTRATOR_ROLE_NAME = 'Administrator';
  SALES_MANAGER_ROLE_NAME = 'SalesManager';
  isAuthenticated = false;
  isAdmin = false;
  isSalesManager = false;
  userEmail = '';
  componentDestroyed$ = new Subject<void>();

  constructor(
    private translateService: TranslateService,
    private matSnackBar: MatSnackBar,
    private router: Router,
    private authenticationService: AuthenticationService) { }

  ngOnInit(): void {
    this.authenticationService.authUser.pipe(takeUntil(this.componentDestroyed$))
      .subscribe((user) => {
        if (user) {
          this.isAuthenticated = true;
          this.userEmail = user.profile.email!;
          const userRoles  = user.profile['role'] ?? user.profile['roles'];
          let roleNames = new Array<string>();

          if(userRoles) {
            if ( typeof userRoles ===  'string') {
              roleNames.push(userRoles);
            } else if (Array.isArray(userRoles)) {
              roleNames = userRoles;
            }
          }

          this.isAdmin = roleNames.indexOf(this.ADMINISTRATOR_ROLE_NAME) >= 0;
          this.isSalesManager = roleNames.indexOf(this.SALES_MANAGER_ROLE_NAME) >= 0;
        } else {
          this.isAdmin = this.isAuthenticated = this.isSalesManager = false;
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

  login() {
    this.authenticationService.login().catch((error) => {
      this.translateService.get('errors.service-unavailable')
          .pipe(take(1))
          .subscribe(translated=>this.matSnackBar.open(translated , 'Close', { duration: 3000, panelClass: ['error-snack-bar'] }));
      }
    );
  }
}
