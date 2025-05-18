import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { OrderingService } from '../services/ordering.service';
import { BasketClient, CheckoutDTO } from '../services/api/basket.api.client';
import { Router } from '@angular/router';
import { lastValueFrom, take } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html'
})
export class CheckoutComponent implements OnInit {

  checkoutForm!: FormGroup;
  total = 0;
  isCheckingOut = false;

  constructor(private basketClient: BasketClient,
              private orderingService: OrderingService,
              private router: Router,
              private location: Location,
              private translateService: TranslateService,
              private matSnackBar: MatSnackBar){}

  ngOnInit(): void {
    this.checkoutForm = new FormGroup({
      shippingAddress: new FormControl('',[Validators.required])
    });

    this.orderingService.onBasketItemsChanged.pipe(take(1))
      .subscribe((items)=> {
        this.total = items.reduce((acc,current) => acc +(current.qty!  * current.price!), 0);
      });
  }

  async onSubmit() {
    if (!this.checkoutForm.valid) {
      return;
    }

    const checkOutDto = new CheckoutDTO({shippingAddress: this.checkoutForm.value['shippingAddress']});

    const checkOut$ = this.basketClient.checkOut(checkOutDto);
    await lastValueFrom(checkOut$);

    this.orderingService.clearBasket();

    //Wait For 20 sec

    let loop = 20;
    let orderPlaced = false;
    this.isCheckingOut = true;

    while (loop-- > 0) {
      await this.sleep(1000);

      const basket$ = this.basketClient.getBasket();
      const result = await lastValueFrom(basket$);

      if ( result.items && result.items?.length === 0 ) {
        orderPlaced = true;
        break;
      }
    }

    this.isCheckingOut = false;

    if(!orderPlaced) {
      const message$ = this.translateService.get('orders.errors.checkout-failed');
      const message = await lastValueFrom(message$);
      this.matSnackBar.open(message , 'Close', { duration: 3000, panelClass: ['error-snack-bar'] });
      await this.sleep(3000);
    }

    await this.orderingService.initBasket();
    await this.router.navigate(['/']);
  }

  back() {
    this.location.back();
  }

  sleep(timeout: number) : Promise<void> {
    return new Promise(resolve => setTimeout(resolve, timeout));
  }
}
