import { Component, computed, HostListener, inject, signal } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { take } from 'rxjs';
import { AuthenticationService } from '../auth/authentication.service';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    imports: [
      CommonModule,
      RouterModule,
      TranslateModule,
      MatToolbarModule,
      MatButtonModule,
      MatMenuModule,
      MatIconModule
    ]
})
export class HeaderComponent {
  MAX_SMALL_WIDTH = 430;
  ADMINISTRATOR_ROLE_NAME = 'Administrator';
  SALES_MANAGER_ROLE_NAME = 'SalesManager';

  authenticationService = inject(AuthenticationService);
  private translateService = inject(TranslateService);
  private matSnackBar = inject(MatSnackBar);
  private router = inject(Router);

  screenWidth = signal(window.innerWidth);
  isSmallScreen = signal(window.innerWidth < this.MAX_SMALL_WIDTH);
  authUser = signal<any>(null);

  // Properties updated when authUser changes
  isAuthenticated = computed(()=> this.authenticationService.isAuthenticated());
  userEmail = computed(()=> this.authenticationService.userEmail());
  isAdmin = computed(()=> this.authenticationService.isAdmin());
  isSalesManager = computed(()=> this.authenticationService.isSalesManager());

  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth.set(event.target.innerWidth);
    this.isSmallScreen.set(event.target.innerWidth < this.MAX_SMALL_WIDTH);
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/']);
  }

  login() {
    this.authenticationService.login().catch((error) => {
      this.translateService.get('errors.service-unavailable')
          .pipe(take(1))
          .subscribe(translated=>this.matSnackBar.open(translated , 'Close', { duration: 3000 }));
      }
    );
  }
}
