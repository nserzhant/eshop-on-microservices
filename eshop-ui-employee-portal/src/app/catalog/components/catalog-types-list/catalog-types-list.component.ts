import { Component, DestroyRef, inject, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { TranslateModule } from '@ngx-translate/core';
import { merge, of as observableOf, Subject} from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap} from 'rxjs/operators';
import { CatalogDomainErrorDTO, CatalogTypeClient, CatalogTypeDTO, CatalogTypeReadModel, ICatalogTypeDTO, OrderByDirections } from '../../services/api/catalog.api.client';
import { CatalogApiErrorsSummaryComponent } from '../catalog-api-errors-summary/catalog-api-errors-summary.component';
import { MATERIAL_COMPONENTS_IMPORTS } from 'src/app/shared/material-imports';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
    selector: 'app-catalog-types-list',
    templateUrl: './catalog-types-list.component.html',
    imports: [
      CommonModule,
      FormsModule,
      TranslateModule,
      CatalogApiErrorsSummaryComponent,
      ...MATERIAL_COMPONENTS_IMPORTS
    ]
})
export class CatalogTypesListComponent {
  displayedColumns: string[] = ['Type', 'Edit', 'Delete'];
  refreshDataSubject$ = new Subject<void>();

  isLoadingResults = signal(true);
  data = signal<CatalogTypeReadModel[]>([]);
  resultsLength = signal(0);
  apiError = signal<CatalogDomainErrorDTO | null>(null);
  selectedType = signal<ICatalogTypeDTO | null>(null);
  selectedTypeId = signal<string | null>(null);
  isTypeSaving = signal(false);

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  catalogTypeClient = inject(CatalogTypeClient);
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
        return this.catalogTypeClient.getCatalogTypes(
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
        return data.catalogTypes ?? [];
      }),
    )
    .subscribe(data => this.data.set(data));
  }

  editType(type: CatalogTypeReadModel) {
    this.selectedType.set({ ...type});
    this.selectedTypeId.set(type.id!);
    this.apiError.set(null);
  }

  saveType(form: NgForm) {
    if (!form.valid) {
      return;
    }

    this.isTypeSaving.set(true);
    const typeToSave = new CatalogTypeDTO(this.selectedType()!);

    const saveObservable = this.selectedTypeId() === null ?
      this.catalogTypeClient.createCatalogType(typeToSave) :
      this.catalogTypeClient.updateCatalogType(this.selectedTypeId()!, typeToSave);

    saveObservable.subscribe({
      error: (e) => {
        this.isTypeSaving.set(false);
        this.apiError.set(e);
      },
      complete: () => {
        this.isTypeSaving.set(false);
        this.selectedType.set(null);
        this.refreshDataSubject$.next();
        form.resetForm();
        this.apiError.set(null);
      }
    });
  }

  deleteType(type: CatalogTypeReadModel) {
    this.isLoadingResults.set(true);
    this.catalogTypeClient.deleteCatalogType(type.id!).subscribe({
      error: (e) => {
        this.isLoadingResults.set(false);
        this.apiError.set(e);
      },
      complete: () => {
        this.isTypeSaving.set(false);
        this.refreshDataSubject$.next();
        this.apiError.set(null);
      }
    });
  }

  close(form: NgForm) {
    this.selectedType.set(null);
    form.resetForm();
    this.apiError.set(null);
  }

  openCreateType() {
    this.apiError.set(null);
    this.selectedType.set({ type: ''});
    this.selectedTypeId.set(null);
  }

  updateCatalogType(updateCatalogTypeName: string) {
    if(this.selectedType()!==null) {
      this.selectedType.set({...this.selectedType(),  type: updateCatalogTypeName });
    }
  }
}
