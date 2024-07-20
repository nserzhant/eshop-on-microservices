import { Injectable } from '@angular/core';
import { BehaviorSubject, lastValueFrom, map, Observable } from 'rxjs';
import { CatalogItemReadModel } from './api/catalog.api.client';
import { BasketClient, BasketItem, CustomerBasket } from './api/basket.api.client';

@Injectable({
  providedIn: 'root'
})
export class OrderingService {
  private readonly BASKET_STORAGE_KEY = 'basket';
  private basketSubject$ = new BehaviorSubject<CustomerBasket>(new CustomerBasket({items: []}));
  private storage: Storage;

  get onBasketItemsChanged() : Observable<BasketItem[]> {
    return this.basketSubject$.asObservable().pipe(map(basket => basket.items ?? []));
  }

  get isBasketSaved(): boolean {
    return this.basketSubject$.value.id !== undefined;
  }

  constructor(private basketClient: BasketClient) {
    this.storage = sessionStorage;

    const storageBasket = this.storage.getItem(this.BASKET_STORAGE_KEY);

    if (storageBasket) {
        const parsedBasket = JSON.parse(storageBasket) as CustomerBasket;

        if(parsedBasket && parsedBasket.items) {
          this.basketSubject$.next(new CustomerBasket({
            items: parsedBasket.items.map( item => new BasketItem(item))
          }));
        }
    }
  }

  addItemToBasket( catalogItem : CatalogItemReadModel) {
    const currentBasket = this.basketSubject$.value;
    const existedItem = currentBasket.items?.find((bi)=>bi.catalogItemId === catalogItem.id);

    if (existedItem !== undefined) {
      existedItem.qty!++;
    } else {
      const basketItem = new BasketItem({
        catalogItemId: catalogItem.id,
        itemName: catalogItem.name,
        brandName: catalogItem.catalogBrand?.brand,
        typeName: catalogItem.catalogType?.type,
        qty: 1,
        price: catalogItem.price,
        pictureUri : catalogItem.pictureUri,
        description: catalogItem.description
      });

      currentBasket.items?.push(basketItem);
    }

    this.saveBasket(currentBasket.items!);
  }

  async initBasket() {
    const savedBasket = await lastValueFrom(this.basketClient.getBasket());
    let currentBasket = this.basketSubject$.value;
    const doesSavedItemsExists = savedBasket.items !== undefined && savedBasket.items.length > 0;
    const doesCurrentItemsExists = currentBasket.items !== undefined && currentBasket.items.length > 0;

    if( doesCurrentItemsExists && !doesSavedItemsExists ) {
      currentBasket.id = savedBasket.id;
      await lastValueFrom(this.basketClient.saveBasket(currentBasket));
    } else {
      currentBasket = savedBasket;
    }

    this.basketSubject$.next(currentBasket);
  }

  async saveBasket(items: BasketItem[]) {
    const basket = new CustomerBasket(this.basketSubject$.value);
    basket.items = items.slice();

    this.basketSubject$.next(basket);

    if (basket.id !== undefined) {
      await lastValueFrom(this.basketClient.saveBasket(basket));
      this.storage.removeItem(this.BASKET_STORAGE_KEY);
    } else {
      this.storage.setItem(this.BASKET_STORAGE_KEY, JSON.stringify(basket));
    }
  }

  clearBasket() {
    this.basketSubject$.next(new CustomerBasket({items: []}));
  }
}
