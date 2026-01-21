import { Component, computed, HostListener, inject, OnInit, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { OrderClient, OrderReadModel } from '../services/api/ordering.api.client';
import { lastValueFrom } from 'rxjs';
import { MATERIAL_TABLE_IMPORTS, MATERIAL_COMMON_IMPORTS } from '../shared/material-imports';

@Component({
    selector: 'app-orders',
    templateUrl: './orders.component.html',
    imports: [
      TranslateModule,
      ...MATERIAL_TABLE_IMPORTS,
      ...MATERIAL_COMMON_IMPORTS
    ]
})
export class OrdersComponent implements OnInit {
  MAX_SMALL_WIDTH = 520;

  private orderClient = inject(OrderClient);
  screenWidth = signal(window.innerWidth);
  orders = signal<OrderReadModel[]>([]);

  isSmallScreen = computed(() => this.screenWidth() < this.MAX_SMALL_WIDTH);
  displayedColumns = computed(() => {
    return this.isSmallScreen() ?  ['image','name','qty','price'] : ['image','name','brand','type','qty','price'];
  });

  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth.set(event.target.innerWidth);
  }

  async ngOnInit() {
    const orders$ = this.orderClient.getOrders();
    const result = await lastValueFrom(orders$);
    this.orders.set(result.orders ?? []);
  }
}
