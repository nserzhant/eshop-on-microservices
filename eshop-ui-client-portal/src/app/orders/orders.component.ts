import { Component, OnInit } from '@angular/core';
import { OrderClient, OrderReadModel } from '../services/api/ordering.api.client';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit  {
  items: OrderReadModel[]= [];
  displayedColumns: string[] = ['image','name','brand','type','qty','price']
  constructor(private orderClient: OrderClient){
  }

  async ngOnInit() {
    const result = await lastValueFrom(this.orderClient.getOrders());
    this.items = result.orders ?? [];
  }
}
