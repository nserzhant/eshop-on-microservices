import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthenticationService } from './authentication.service';
import { BasketService } from '../services/basket.service';

@Component({
    selector: 'login-callback',
    template: `<p>{{ 'process-login-callback' | translate }}</p>`,
    imports: [TranslateModule]
})
export class LoginCallbackComponent {

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private basketService: BasketService) {

    this.authenticationService.signinCallback().then(()=> {
        this.router.navigate(['']);
        this.authenticationService.handleUserLoggedIn().then(()=> {
          this.basketService.initBasket();
        });
      }
    );
   }
}
