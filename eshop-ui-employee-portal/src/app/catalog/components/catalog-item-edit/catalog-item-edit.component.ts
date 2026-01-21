import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { Location } from '@angular/common';
import { CatalogBrandClient, CatalogBrandReadModel, CatalogDomainErrorDTO, CatalogItemClient, CatalogItemDTO, CatalogItemReadModel, CatalogTypeClient, CatalogTypeReadModel, OrderByDirections } from '../../services/api/catalog.api.client';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { TextFieldModule } from '@angular/cdk/text-field';
import { CatalogApiErrorsSummaryComponent } from '../catalog-api-errors-summary/catalog-api-errors-summary.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MATERIAL_COMPONENTS_IMPORTS } from 'src/app/shared/material-imports';

@Component({
    selector: 'app-catalog-item-edit',
    templateUrl: './catalog-item-edit.component.html',
    imports: [
      ReactiveFormsModule,
      TranslateModule,
      ...MATERIAL_COMPONENTS_IMPORTS,
      TextFieldModule,
      CatalogApiErrorsSummaryComponent
    ]
})
export class CatalogItemEditComponent implements OnInit {
  editItemForm!: FormGroup;

  private catalogItemClient = inject(CatalogItemClient);
  private catalogBrandClient = inject(CatalogBrandClient);
  private catalogTypeClient = inject(CatalogTypeClient);
  private route = inject(ActivatedRoute);
  private location = inject(Location);

  catalogItem = signal<CatalogItemReadModel | null>(null);
  catalogTypes = signal<CatalogTypeReadModel[]>([]);
  catalogBrands = signal<CatalogBrandReadModel[]>([]);
  isCatalogItemSaving = signal(false);
  apiError = signal<CatalogDomainErrorDTO | null>(null);
  destroyRef$ = inject(DestroyRef);

  ngOnInit(): void {
    this.editItemForm = new FormGroup({
      name: new FormControl('',[Validators.required]),
      brandId: new FormControl('', [Validators.required]),
      typeId: new FormControl('', [Validators.required]),
      price: new FormControl('', [Validators.pattern(/^[0-9]{0,4}(\.[0-9]{0,2})?$/)]),
      description: new FormControl(''),
      pictureUri: new FormControl(''),
      availableQty: new FormControl('',[Validators.required, Validators.pattern(/^([0-9])/)])
    });

    this.route.params.pipe(takeUntilDestroyed(this.destroyRef$)).subscribe((params: Params) => {
      const id = params['catalogItemId'];

      if(id) {
        this.loadItem(id);
      }
    });

    this.loadBrands();
    this.loadTypes();
  }

  async loadTypes() {
    const items$ = this.catalogTypeClient.getCatalogTypes(OrderByDirections.ASC);
    const result = await lastValueFrom(items$);
    this.catalogTypes.set(result.catalogTypes!);
  }

  async loadBrands() {
    const items$ = this.catalogBrandClient.getCatalogBrands(OrderByDirections.ASC);
    const result = await lastValueFrom(items$);
    this.catalogBrands.set(result.catalogBrands!);
  }

  async loadItem(catalogItemId: string) {
    const item$ = this.catalogItemClient.getCatalogItem(catalogItemId);
    const itemData = await lastValueFrom(item$);

    this.catalogItem.set(itemData);
    this.editItemForm.setValue({
      name: itemData.name,
      typeId: itemData.catalogTypeId,
      brandId: itemData.catalogBrandId,
      price: itemData.price,
      description : itemData.description,
      pictureUri: itemData.pictureUri,
      availableQty: itemData.availableQty
    });
  }

  async onSubmit() {
    if (!this.editItemForm.valid) {
      return;
    }

    this.isCatalogItemSaving.set(true);

    const currentItem = this.catalogItem();
    const itemToSave = new CatalogItemDTO({...currentItem, ...this.editItemForm.value});

    const saveObservable = currentItem === null ?
      this.catalogItemClient.createCatalogItem(itemToSave) :
      this.catalogItemClient.updateCatalogItem(currentItem.id!, itemToSave);

    try {
      await lastValueFrom(saveObservable);
      this.apiError.set(null);
      this.location.back();
    } catch (e) {
      this.apiError.set(e as CatalogDomainErrorDTO);
    } finally {
      this.isCatalogItemSaving.set(false);
    }
  }

  back() {
    this.location.back();
  }
}
