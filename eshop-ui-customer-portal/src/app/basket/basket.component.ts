import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderingService } from '../services/ordering.service';
import { BasketItem } from '../services/api/basket.api.client';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html'
})
export class BasketComponent implements OnInit, OnDestroy {
  items: BasketItem[] = [];
  displayedColumns: string[] = ['itemName', 'qty', 'price', 'remove'];
  readonly panelOpenState = signal(false);
  componentDestroyed$ = new Subject<void>();

  get canCheckout(): boolean {
    return this.orderingService.isBasketSaved && this.items.length > 0;
  }

  constructor(private orderingService: OrderingService) {
  }

  ngOnInit(): void {
    this.orderingService.onBasketItemsChanged.pipe(takeUntil(this.componentDestroyed$))
      .subscribe((items)=> {
        this.items = items;
      });
  }

  ngOnDestroy(): void {
    this.componentDestroyed$.next();
  }

  remove(item: BasketItem) {
    const index = this.items.indexOf(item);

    this.items.splice(index, 1);
    this.orderingService.saveBasket(this.items);
    this.items = this.items.slice();
  }

  qtyChanged(item: BasketItem, newQty: number) {
    item.qty = newQty;
    this.orderingService.saveBasket(this.items);
  }
}
