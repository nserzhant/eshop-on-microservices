import { Routes } from '@angular/router';
import { LoginCallbackComponent } from './auth/login-callback.component';
import { CatalogComponent } from './catalog/catalog.component';
import { BasketComponent } from './basket/basket.component';
import { OrdersComponent } from './orders/orders.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { authGuard } from './auth/auth.guard.';

export const routes: Routes = [
  {
    path: '',
    component: CatalogComponent
  },
  {
    path: 'login-callback',
    component: LoginCallbackComponent
  },
  {
    path: 'basket',
    component: BasketComponent
  },
  {
    path: 'orders',
    component: OrdersComponent,
    canActivate: [authGuard]
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
    canActivate: [authGuard]
  }
];
