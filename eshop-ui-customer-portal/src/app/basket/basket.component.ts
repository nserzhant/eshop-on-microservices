import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { BasketService } from '../services/basket.service';
import { BasketItem, CustomerBasket } from '../services/api/basket.api.client';
import { MATERIAL_TABLE_IMPORTS, MATERIAL_FORM_IMPORTS } from '../shared/material-imports';

@Component({
    selector: 'app-basket',
    templateUrl: './basket.component.html',
    styleUrl: './basket.component.css',
    imports: [
      RouterLink,
      FormsModule,
      TranslateModule,
      ...MATERIAL_TABLE_IMPORTS,
      ...MATERIAL_FORM_IMPORTS
    ]
})
export class BasketComponent {
  displayedColumns: string[] = ['itemName', 'qty', 'price', 'remove'];

  private basketService = inject(BasketService);
  readonly panelOpenState = signal(false);
  items = computed(() => this.basketService.basket().items ?? []);
  canCheckout = computed(() => this.basketService.isBasketSaved() && this.items().length > 0);

  remove(item: BasketItem) {
    this.basketService.removeItemFromBasket(item);
  }

  qtyChanged(item: BasketItem, newQty: number) {
    item.qty = newQty;
    this.basketService.saveBasket(new CustomerBasket({...this.basketService.basket()}));
  }
}
