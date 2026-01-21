import { computed, Injectable, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { CatalogItemReadModel } from './api/catalog.api.client';
import { BasketClient, BasketItem, CustomerBasket } from './api/basket.api.client';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  private readonly BASKET_STORAGE_KEY = 'basket';
  private storage: Storage;
  private _basket = signal<CustomerBasket>(new CustomerBasket({items: []}));

  basket = this._basket.asReadonly();
  isBasketSaved = computed(()=> this.basket().id !== undefined);

  constructor(private basketClient: BasketClient) {
    this.storage = sessionStorage;

    const storageBasketJSON = this.storage.getItem(this.BASKET_STORAGE_KEY);

    if (storageBasketJSON) {
      const parsedBasket = JSON.parse(storageBasketJSON) as CustomerBasket;

      if(parsedBasket && parsedBasket.items) {
        this._basket.set(new CustomerBasket({
          id: parsedBasket.id,
          items: parsedBasket.items.map( item => new BasketItem(item))
        }));
      }
    }
  }

  addItemToBasket( catalogItem : CatalogItemReadModel) {
    const updatedBasket = {...this.basket(), items: this.basket().items?.slice()};
    const existedItem = updatedBasket.items?.find((bi) => bi.catalogItemId === catalogItem.id);

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

      updatedBasket.items?.push(basketItem);
    }

    this.saveBasket(new CustomerBasket(updatedBasket));
  }

  removeItemFromBasket(item: BasketItem) {
    const index = this.basket().items?.indexOf(item);
    const updatedBasket = {...this.basket(), items: this.basket().items?.filter((itm,ix) => ix !== index)};

    this.saveBasket(new CustomerBasket(updatedBasket));
  }

  async initBasket() {
    const basket$ = this.basketClient.getBasket();
    const savedBasket = await lastValueFrom(basket$);
    let localBasket = this.basket();
    const doesSavedBasketItemsExists = savedBasket.items !== undefined && savedBasket.items.length > 0;
    const doesLocalBasketItemsExists = localBasket.items !== undefined && localBasket.items.length > 0;

    if( doesLocalBasketItemsExists && !doesSavedBasketItemsExists ) {
      localBasket.id = savedBasket.id;

      const saveBasket$ = this.basketClient.saveBasket(localBasket);
      await lastValueFrom(saveBasket$);
    } else {
      localBasket = savedBasket;
    }

    this._basket.set(localBasket);
  }

  async saveBasket(basket: CustomerBasket) {
    this._basket.set(basket);

    if (basket.id !== undefined) {
      const saveBasket$ = this.basketClient.saveBasket(basket);
      await lastValueFrom(saveBasket$);

      this.storage.removeItem(this.BASKET_STORAGE_KEY);
    }

    this.storage.setItem(this.BASKET_STORAGE_KEY, JSON.stringify(basket));
  }

  clearBasket() {
    this._basket.set(new CustomerBasket({items: []}));
  }
}
