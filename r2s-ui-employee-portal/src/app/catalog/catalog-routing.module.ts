import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CatalogBrandsListComponent } from "./components/catalog-brands-list/catalog-brands-list.component";
import { CatalogItemEditComponent } from "./components/catalog-item-edit/catalog-item-edit.component";
import { CatalogItemsListComponent } from "./components/catalog-items-list/catalog-items-list.component";
import { CatalogTypesListComponent } from "./components/catalog-types-list/catalog-types-list.component";

const routes: Routes = [
  {
    path: 'catalog',
    children : [
      {
        path: 'items',
        component: CatalogItemsListComponent
      },
      {
        path: 'items/edit/:catalogItemId',
        component: CatalogItemEditComponent
      },
      {
        path: 'items/new',
        component: CatalogItemEditComponent
      },
      {
        path:'brands',
        component: CatalogBrandsListComponent
      },
      {
        path: 'types',
        component: CatalogTypesListComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class CatalogRoutingModule {}
