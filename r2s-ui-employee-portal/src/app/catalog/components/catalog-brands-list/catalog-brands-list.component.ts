import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, of as observableOf, Subject} from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap} from 'rxjs/operators';
import { CatalogBrandClient, CatalogBrandDTO, CatalogBrandReadModel, CatalogDomainErrorDTO, ICatalogBrandDTO, OrderByDirections } from '../../services/api/catalog.api.client';

@Component({
  selector: 'app-catalog-brands-list',
  templateUrl: './catalog-brands-list.component.html'
})
export class CatalogBrandsListComponent implements AfterViewInit {
  displayedColumns: string[] = ['Brand', 'Edit', 'Delete'];
  refreshDataSubject$ = new Subject<void>();
  isLoadingResults = true;
  data: CatalogBrandReadModel[] = [];
  resultsLength = 0;

  selectedBrand: ICatalogBrandDTO | null = null;
  selectedBrandId: string | null = null;

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  isBrandSaving = false;
  apiError : CatalogDomainErrorDTO | null = null;

  ngOnInit(): void {
  }

  constructor(private catalogBrandClient: CatalogBrandClient) {}

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page, this.refreshDataSubject$)
    .pipe(
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.isLoadingResults = true;
        const orderByDirection = OrderByDirections[this.sort.direction.toUpperCase() as keyof typeof OrderByDirections];
        return this.catalogBrandClient.getCatalogBrands(
          orderByDirection,
          this.paginator.pageIndex,
          this.paginator.pageSize).pipe(catchError(() => observableOf(null)));
      }),
      map(data => {
        // Flip flag to show that loading has finished.
        this.isLoadingResults = false;

        if (data === null) {
          return [];
        }

        // Only refresh the result length if there is new data. In case of rate
        // limit errors, we do not want to reset the paginator to zero, as that
        // would prevent users from re-triggering requests.
        this.resultsLength = data.totalCount ?? 0;
        return data.catalogBrands ?? [];
      }),
    )
    .subscribe(data => (this.data = data));
  }

  editBrand(brand: CatalogBrandReadModel) {
    this.selectedBrand = { ...brand};
    this.selectedBrandId = brand.id!;
  }

  saveBrand(form: NgForm) {
    if (!form.valid) {
      return;
    }

    this.isBrandSaving = true;
    const brandToSave = new CatalogBrandDTO(this.selectedBrand!);

    var saveObservable = this.selectedBrandId === null ?
      this.catalogBrandClient.createCatalogBrand(brandToSave) :
      this.catalogBrandClient.updateCatalogBrand(this.selectedBrandId, brandToSave);

    saveObservable.subscribe({
      error: (e) => {
        this.isBrandSaving = false;
        this.apiError = e;
      },
      complete: () => {
        this.isBrandSaving = false;
        this.selectedBrand = null;
        this.refreshDataSubject$.next();
        form.resetForm();
        this.apiError = null;
      }
    });
  }

  deleteBrand(brand: CatalogBrandReadModel) {
    this.isLoadingResults = true;
    this.catalogBrandClient.deleteCatalogBrand(brand.id!).subscribe({
      error: (e) => {
        this.isLoadingResults = false;
        this.apiError = e;
      },
      complete: () => {
        this.isBrandSaving = false;
        this.refreshDataSubject$.next();
        this.apiError = null;
      }
    });
  }

  close(form: NgForm) {
    this.selectedBrand = null;
    form.resetForm();
    this.apiError = null;
  }

  openCreateBrand() {
    this.selectedBrand = { brand: ''};
    this.selectedBrandId = null;
  }
}
