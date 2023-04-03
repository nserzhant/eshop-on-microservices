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
  isAuthenticated = false;
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
        } else {
          this.isAuthenticated = false;
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
