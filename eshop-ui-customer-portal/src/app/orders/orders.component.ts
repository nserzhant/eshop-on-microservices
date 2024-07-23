import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { OrderClient, OrderReadModel } from '../services/api/ordering.api.client';
import { BehaviorSubject, lastValueFrom, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit, OnDestroy  {

  MAX_SMALL_WIDTH = 520;
  componentDestroyed$ = new Subject<void>();
  screenWidth$ = new BehaviorSubject<number>(window.innerWidth);
  isSmallScreen = false;

  items: OrderReadModel[]= [];

  get displayedColumns(): string[] {
    return this.isSmallScreen ?  ['image','name','qty','price'] : ['image','name','brand','type','qty','price'];
  }

  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth$.next(event.target.innerWidth);
  }

  constructor(private orderClient: OrderClient){
  }

  async ngOnInit() {
    const orders$ = this.orderClient.getOrders();
    const result = await lastValueFrom(orders$);
    this.items = result.orders ?? [];

    this.screenWidth$.asObservable().pipe(takeUntil(this.componentDestroyed$)).subscribe(width => {
         if (width < this.MAX_SMALL_WIDTH) {
          this.isSmallScreen = true;
        }
        else if (width >  this.MAX_SMALL_WIDTH) {
          this.isSmallScreen = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.componentDestroyed$.next();
  }
}
