import { Routes } from '@angular/router';
import { authGuard } from '../auth/auth.guard';
import { CatalogItemsListComponent } from './components/catalog-items-list/catalog-items-list.component';
import { CatalogItemEditComponent } from './components/catalog-item-edit/catalog-item-edit.component';
import { CatalogBrandsListComponent } from './components/catalog-brands-list/catalog-brands-list.component';
import { CatalogTypesListComponent } from './components/catalog-types-list/catalog-types-list.component';

export const catalogRoutes: Routes = [
  {
    path: 'items',
    component: CatalogItemsListComponent,
    canActivate: [authGuard],
    data: { roles: ['SalesManager'] }
  },
  {
    path: 'items/edit/:catalogItemId',
    component: CatalogItemEditComponent,
    canActivate: [authGuard],
    data: { roles: ['SalesManager'] }
  },
  {
    path: 'items/new',
    component: CatalogItemEditComponent,
    canActivate: [authGuard],
    data: { roles: ['SalesManager'] }
  },
  {
    path: 'brands',
    component: CatalogBrandsListComponent,
    canActivate: [authGuard],
    data: { roles: ['SalesManager'] }
  },
  {
    path: 'types',
    component: CatalogTypesListComponent,
    canActivate: [authGuard],
    data: { roles: ['SalesManager'] }
  }
];
