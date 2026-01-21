import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Location } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BasketService } from '../services/basket.service';
import { BasketClient, CheckoutDTO } from '../services/api/basket.api.client';
import { lastValueFrom } from 'rxjs';
import { MATERIAL_FORM_IMPORTS, MATERIAL_COMMON_IMPORTS } from '../shared/material-imports';

@Component({
    selector: 'app-checkout',
    templateUrl: './checkout.component.html',
    imports: [
      ReactiveFormsModule,
      TranslateModule,
      ...MATERIAL_FORM_IMPORTS,
      ...MATERIAL_COMMON_IMPORTS
    ]
})
export class CheckoutComponent implements OnInit {
  private basketClient = inject(BasketClient);
  private basketService= inject(BasketService);
  private router = inject(Router);
  private location = inject(Location);
  private translateService = inject(TranslateService);
  private matSnackBar = inject(MatSnackBar);

  checkoutForm!: FormGroup;
  items = computed(() => this.basketService.basket().items ?? []);
  totalPrice = computed(() => {
    return this.items().reduce((acc,current) => acc + (current.qty! * current.price!), 0);
  });
  isCheckingOut = signal(false);

  ngOnInit(): void {
    this.checkoutForm = new FormGroup({
      shippingAddress: new FormControl('',[Validators.required])
    });
  }

  async onSubmit() {
    if (!this.checkoutForm.valid) {
      return;
    }

    const checkOutDto = new CheckoutDTO({shippingAddress: this.checkoutForm.value['shippingAddress']});
    const checkOut$ = this.basketClient.checkOut(checkOutDto);

    await lastValueFrom(checkOut$);
    this.basketService.clearBasket();

    //Wait For 20 sec

    let loop = 20;
    let orderPlaced = false;
    this.isCheckingOut.set(true);

    while (loop-- > 0) {
      await this.sleep(1000);

      const basket$ = this.basketClient.getBasket();
      const result = await lastValueFrom(basket$);

      if ( result.items && result.items?.length === 0 ) {
        orderPlaced = true;
        break;
      }
    }

    this.isCheckingOut.set(false);

    if(!orderPlaced) {
      const message$ = this.translateService.get('orders.errors.checkout-failed');
      const message = await lastValueFrom(message$);
      this.matSnackBar.open(message , 'Close', { duration: 3000 });
      await this.sleep(3000);
    }

    await this.basketService.initBasket();
    await this.router.navigate(['/']);
  }

  back() {
    this.location.back();
  }

  sleep(timeout: number) : Promise<void> {
    return new Promise(resolve => setTimeout(resolve, timeout));
  }
}
