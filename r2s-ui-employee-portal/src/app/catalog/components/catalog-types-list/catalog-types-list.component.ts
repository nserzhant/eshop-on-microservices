import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, of as observableOf, Subject} from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap} from 'rxjs/operators';
import { CatalogTypeClient, CatalogTypeDTO, CatalogTypeReadModel, ICatalogTypeDTO, OrderByDirections } from '../../services/api/catalog.api.client';

@Component({
  selector: 'app-catalog-types-list',
  templateUrl: './catalog-types-list.component.html',
  styleUrls: ['./catalog-types-list.component.css']
})
export class CatalogTypesListComponent {
  displayedColumns: string[] = ['Type', 'Edit', 'Delete'];
  refreshDataSubject$ = new Subject<void>();
  isLoadingResults = true;
  data: CatalogTypeReadModel[] = [];
  resultsLength = 0;

  selectedType: ICatalogTypeDTO | null = null;
  selectedTypeId: string | null = null;

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  isTypeSaving = false;

  ngOnInit(): void {
  }

  constructor(private catalogTypeClient: CatalogTypeClient) {}

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page, this.refreshDataSubject$)
    .pipe(
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.isLoadingResults = true;
        const orderByDirection = OrderByDirections[this.sort.direction.toUpperCase() as keyof typeof OrderByDirections];
        return this.catalogTypeClient.getCatalogTypes(
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
        return data.catalogTypes ?? [];
      }),
    )
    .subscribe(data => (this.data = data));
  }

  editType(type: CatalogTypeReadModel) {
    this.selectedType = { ...type};
    this.selectedTypeId = type.id!;
  }

  saveType(form: NgForm) {
    if (!form.valid) {
      return;
    }

    this.isTypeSaving = true;
    const typeToSave = new CatalogTypeDTO(this.selectedType!);

    var saveObservable = this.selectedTypeId === null ?
      this.catalogTypeClient.createCatalogType(typeToSave) :
      this.catalogTypeClient.updateCatalogType(this.selectedTypeId, typeToSave);

    saveObservable.subscribe({
      error: (e) => {
        this.isTypeSaving = false;
      },
      complete: () => {
        this.isTypeSaving = false;
        this.selectedType = null;
        this.refreshDataSubject$.next();
        form.reset();
      }
    });
  }

  deleteType(type: CatalogTypeReadModel) {
    this.isLoadingResults = true;
    this.catalogTypeClient.deleteCatalogType(type.id!).subscribe({
      error: (e) => {
        this.isLoadingResults = false;
      },
      complete: () => {
        this.isTypeSaving = false;
        this.refreshDataSubject$.next();
      }
    });
  }

  close(form: NgForm) {
    this.selectedType = null;
    form.reset();
  }

  openCreateType() {
    this.selectedType = { type: ''};
    this.selectedTypeId = null;
  }
}
