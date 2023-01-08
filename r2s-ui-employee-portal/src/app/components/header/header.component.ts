import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Subject, take, takeUntil } from 'rxjs';
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
          const userRoles  = user.profile['role'];
          let roleNames = new Array<string>();

          if(userRoles) {
            if ( typeof userRoles ===  'string') {
              roleNames.push(userRoles);
            } else if (Array.isArray(userRoles)) {
              roleNames = userRoles;
            }
          }
          
          const roles = roleNames?.map(r=> Roles[r as keyof typeof Roles]) || new Array<Roles>();

          if (roles!.indexOf(Roles.Administrator) >= 0) {
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

  login() {
    this.authenticationService.login().catch((error) => {
      this.translateService.get('errors.service-unavailable')
          .pipe(take(1))
          .subscribe(translated=>this.matSnackBar.open(translated , 'Close', { duration: 3000, panelClass: 'error-snack-bar' }));                  
      }
    );
  }
}
