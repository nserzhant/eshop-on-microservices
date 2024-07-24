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

    const storageBasketJSON = this.storage.getItem(this.BASKET_STORAGE_KEY);

    if (storageBasketJSON) {
        const parsedBasket = JSON.parse(storageBasketJSON) as CustomerBasket;

        if(parsedBasket && parsedBasket.items) {
          this.basketSubject$.next(new CustomerBasket({
            items: parsedBasket.items.map( item => new BasketItem(item))
          }));
        }
    }
  }

  addItemToBasket( catalogItem : CatalogItemReadModel) {
    const localBasket = this.basketSubject$.value;
    const existedItem = localBasket.items?.find((bi)=>bi.catalogItemId === catalogItem.id);

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

      localBasket.items?.push(basketItem);
    }

    this.saveBasket(localBasket.items!);
  }

  async initBasket() {
    const basket$ = this.basketClient.getBasket();
    const savedBasket = await lastValueFrom(basket$);
    let localBasket = this.basketSubject$.value;
    const doesSavedBasketItemsExists = savedBasket.items !== undefined && savedBasket.items.length > 0;
    const doesLocalBasketItemsExists = localBasket.items !== undefined && localBasket.items.length > 0;

    if( doesLocalBasketItemsExists && !doesSavedBasketItemsExists ) {
      localBasket.id = savedBasket.id;

      const saveBasket$ = this.basketClient.saveBasket(localBasket);
      await lastValueFrom(saveBasket$);
    } else {
      localBasket = savedBasket;
    }

    this.basketSubject$.next(localBasket);
  }

  async saveBasket(items: BasketItem[]) {
    const basket = new CustomerBasket(this.basketSubject$.value);
    basket.items = items.slice();

    this.basketSubject$.next(basket);

    if (basket.id !== undefined) {
      const saveBasket$ = this.basketClient.saveBasket(basket);
      await lastValueFrom(saveBasket$);

      this.storage.removeItem(this.BASKET_STORAGE_KEY);
    } else {
      this.storage.setItem(this.BASKET_STORAGE_KEY, JSON.stringify(basket));
    }
  }

  clearBasket() {
    this.basketSubject$.next(new CustomerBasket({items: []}));
  }
}
