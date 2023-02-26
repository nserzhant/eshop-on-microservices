import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { Location } from '@angular/common';
import { CatalogBrandClient, CatalogBrandReadModel, CatalogDomainErrorDTO, CatalogItemClient, CatalogItemDTO, CatalogItemReadModel, CatalogTypeClient, CatalogTypeReadModel, ICatalogItemDTO } from '../../services/api/catalog.api.client';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-catalog-item-edit',
  templateUrl: './catalog-item-edit.component.html'
})
export class CatalogItemEditComponent implements OnInit {

  editItemForm!: FormGroup;
  catalogItem: CatalogItemReadModel | null = null;

  isCatalogItemSaving = false;
  apiError : CatalogDomainErrorDTO | null = null;

  catalogTypes: CatalogTypeReadModel[]  = [];
  catalogBrands: CatalogBrandReadModel[] = [];

  constructor(
    private catalogItemClient: CatalogItemClient,
    private catalogBrandClient: CatalogBrandClient,
    private catalogTypeClient: CatalogTypeClient,
    private route : ActivatedRoute,
    private location: Location
  ) {
  }

  ngOnInit(): void {
    this.editItemForm = new FormGroup({
      name: new FormControl('',[Validators.required]),
      brandId: new FormControl('', [Validators.required]),
      typeId: new FormControl('', [Validators.required]),
      price: new FormControl('', [Validators.pattern(/^[0-9]{0,4}(\.[0-9]{0,2})?$/)]),
      description: new FormControl(''),
      pictureUri: new FormControl(''),
      availableQty: new FormControl('',[Validators.pattern(/^([0-9])/)])
    });
    this.route.params.subscribe((params: Params) => {
      const id = params['catalogItemId'];
      this.loadItem(id);
    });
    this.loadBrands();
    this.loadTypes();
  }

  async loadTypes() {
    const items$ = this.catalogTypeClient.getCatalogTypes(undefined, undefined, 1000);
    this.catalogTypes = (await lastValueFrom(items$)).catalogTypes!;
  }

  async loadBrands() {
    const items$ = this.catalogBrandClient.getCatalogBrands(undefined, undefined, 1000);
    this.catalogBrands = (await lastValueFrom(items$)).catalogBrands!;
  }

  async loadItem(catalogItemId: any) {
    const item$ = this.catalogItemClient.getCatalogItem(catalogItemId);
    this.catalogItem  = await lastValueFrom(item$);
    this.editItemForm.setValue({
      name: this.catalogItem.name,
      typeId: this.catalogItem.catalogTypeId,
      brandId: this.catalogItem.catalogBrandId,
      price: this.catalogItem.price,
      description : this.catalogItem.description,
      pictureUri: this.catalogItem.pictureUri,
      availableQty: this.catalogItem.availableQty
    });
  }

  onSubmit() {
    if (!this.editItemForm.valid) {
      return;
    }

    this.isCatalogItemSaving = true;

    const valueToSave: CatalogItemDTO = {...this.catalogItem, ...this.editItemForm.value};
    const itemToSave = new CatalogItemDTO(valueToSave);

    var saveObservable = this.catalogItem === null ?
      this.catalogItemClient.createCatalogItem(itemToSave) :
      this.catalogItemClient.updateCatalogItem(this.catalogItem.id!, itemToSave);

    saveObservable.subscribe({
      error: (e) => {
        this.isCatalogItemSaving = false;
        this.apiError = e;
      },
      complete: () => {
        this.isCatalogItemSaving = false;
        this.apiError = null;
        this.location.back();
      }
    });
  }

  back() {
    this.location.back();
  }
}
