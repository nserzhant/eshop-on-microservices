import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';
import { AuthenticationService } from '../auth/authentication.service';
import { BasketService } from '../services/basket.service';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    imports: [
      RouterLink,
      TranslateModule,
      MatToolbarModule,
      MatButtonModule,
      MatMenuModule,
      MatIconModule
    ]
})
export class HeaderComponent {
  authenticationService = inject(AuthenticationService);
  private translateService = inject(TranslateService);
  private matSnackBar = inject(MatSnackBar);
  private router = inject(Router);
  private basketService = inject(BasketService);

  // Properties updated when authUser changes
  isAuthenticated = computed(()=> this.authenticationService.isAuthenticated());
  userEmail = computed(()=> this.authenticationService.userEmail());

  // Properties updated when basket changes
  basketTotal = computed(() => this.basketService.basket().items?.length ?? 0 );

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
