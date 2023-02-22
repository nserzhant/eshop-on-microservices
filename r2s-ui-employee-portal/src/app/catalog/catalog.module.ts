import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CatalogBrandsListComponent } from './components/catalog-brands-list/catalog-brands-list.component';
import { CatalogTypesListComponent } from './components/catalog-types-list/catalog-types-list.component';
import { CatalogRoutingModule } from './catalog-routing.module';
import { CatalogItemsListComponent } from './components/catalog-items-list/catalog-items-list.component';
import { MaterialModule } from 'src/material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CatalogBrandClient, CatalogItemClient, CatalogTypeClient, CATALOG_API_URL } from './services/api/catalog.api.client';
import { environment } from 'src/environments/environment';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    CatalogBrandsListComponent,
    CatalogTypesListComponent,
    CatalogItemsListComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    CatalogRoutingModule
  ],
  providers : [
    CatalogBrandClient,
    CatalogTypeClient,
    CatalogItemClient,
    {
      provide: CATALOG_API_URL,
      useValue: environment.catalogApi
    }
  ]
})
export class CatalogModule { }
