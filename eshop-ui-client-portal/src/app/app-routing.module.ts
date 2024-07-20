import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginCallbackComponent } from './auth/login-callback.component';
import { CatalogComponent } from './catalog/catalog.component';
import { BasketComponent } from './basket/basket.component';
import { OrdersComponent } from './orders/orders.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { AuthGuard } from './auth/auth-guard.service';

const routes: Routes = [
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
    canActivate: [AuthGuard]
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
