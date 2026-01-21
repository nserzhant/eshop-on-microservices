import { AfterViewInit, Component, DestroyRef, inject, signal, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { TranslateModule } from '@ngx-translate/core';
import { merge, of as observableOf, Subject} from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap} from 'rxjs/operators';
import { CatalogBrandClient, CatalogBrandDTO, CatalogBrandReadModel, CatalogDomainErrorDTO, ICatalogBrandDTO, OrderByDirections } from '../../services/api/catalog.api.client';
import { CatalogApiErrorsSummaryComponent } from '../catalog-api-errors-summary/catalog-api-errors-summary.component';
import { MATERIAL_COMPONENTS_IMPORTS } from 'src/app/shared/material-imports';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
    selector: 'app-catalog-brands-list',
    templateUrl: './catalog-brands-list.component.html',
    imports: [
      FormsModule,
      TranslateModule,
      CatalogApiErrorsSummaryComponent,
      ...MATERIAL_COMPONENTS_IMPORTS
    ]
})
export class CatalogBrandsListComponent implements AfterViewInit {
  displayedColumns: string[] = ['Brand', 'Edit', 'Delete'];
  refreshDataSubject$ = new Subject<void>();

  isLoadingResults = signal(true);
  data = signal<CatalogBrandReadModel[]>([]);
  resultsLength = signal(0);
  apiError = signal<CatalogDomainErrorDTO | null>(null);
  selectedBrand = signal<ICatalogBrandDTO | null>(null);
  selectedBrandId = signal<string | null>(null);
  isBrandSaving = signal(false);

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  catalogBrandClient = inject(CatalogBrandClient);
  destroyRef$ = inject(DestroyRef);

  ngAfterViewInit(): void {
    this.sort.sortChange.pipe(takeUntilDestroyed(this.destroyRef$))
      .subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page, this.refreshDataSubject$)
      .pipe(
        takeUntilDestroyed(this.destroyRef$),
        startWith({}),
        debounceTime(0),
        switchMap((val, index) => {
          this.isLoadingResults.set(true);
          const orderByDirection = OrderByDirections[this.sort.direction.toUpperCase() as keyof typeof OrderByDirections];

          return this.catalogBrandClient.getCatalogBrands(
            orderByDirection,
            this.paginator.pageIndex,
            this.paginator.pageSize).pipe(catchError(() => observableOf(null)));
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults.set(false);

          if (data === null) {
            return [];
          }

          // Only refresh the result length if there is new data. In case of rate
          // limit errors, we do not want to reset the paginator to zero, as that
          // would prevent users from re-triggering requests.
          this.resultsLength.set(data.totalCount ?? 0);
          return data.catalogBrands ?? [];
        })
      )
      .subscribe(data => this.data.set(data));
  }

  editBrand(brand: CatalogBrandReadModel) {
    this.selectedBrand.set({ ...brand});
    this.selectedBrandId.set(brand.id!);
    this.apiError.set(null);
  }

  saveBrand(form: NgForm) {
    if (!form.valid) {
      return;
    }

    this.isBrandSaving.set(true);
    const brandToSave = new CatalogBrandDTO(this.selectedBrand()!);

    const saveObservable = this.selectedBrandId() === null ?
      this.catalogBrandClient.createCatalogBrand(brandToSave) :
      this.catalogBrandClient.updateCatalogBrand(this.selectedBrandId()!, brandToSave);

    saveObservable.subscribe({
      error: (e) => {
        this.isBrandSaving.set(false);
        this.apiError.set(e);
      },
      complete: () => {
        this.isBrandSaving.set(false);
        this.selectedBrand.set(null);
        this.refreshDataSubject$.next();
        form.resetForm();
        this.apiError.set(null);
      }
    });
  }

  deleteBrand(brand: CatalogBrandReadModel) {
    this.isLoadingResults.set(true);
    this.catalogBrandClient.deleteCatalogBrand(brand.id!).subscribe({
      error: (e) => {
        this.isLoadingResults.set(false);
        this.apiError.set(e);
      },
      complete: () => {
        this.isBrandSaving.set(false);
        this.refreshDataSubject$.next();
        this.apiError.set(null);
      }
    });
  }

  close(form: NgForm) {
    form.resetForm();
    this.selectedBrand.set(null);
    this.apiError.set(null);
  }

  openCreateBrand() {
    this.selectedBrand.set({ brand: ''});
    this.selectedBrandId.set(null);
    this.apiError.set(null);
  }

  updateCatalogBrand(updatedBrandName: string) {
    if(this.selectedBrand()!==null) {
      this.selectedBrand.set({...this.selectedBrand(),  brand: updatedBrandName });
    }
  }
}
