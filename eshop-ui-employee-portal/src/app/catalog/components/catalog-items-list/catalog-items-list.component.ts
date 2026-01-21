import { AfterViewInit, afterNextRender, Component, computed, HostListener, signal, ViewChild, inject, DestroyRef } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { merge, of as observableOf, Subject} from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap} from 'rxjs/operators';
import { CatalogDomainErrorDTO, CatalogItemClient, CatalogItemReadModel, ICatalogItemDTO, ListCatalogItemOrderBy, OrderByDirections } from '../../services/api/catalog.api.client';
import { MATERIAL_COMPONENTS_IMPORTS } from 'src/app/shared/material-imports';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
    selector: 'app-catalog-items-list',
    templateUrl: './catalog-items-list.component.html',
    imports: [
      CommonModule,
      TranslateModule,
      ...MATERIAL_COMPONENTS_IMPORTS
    ]
})
export class CatalogItemsListComponent implements AfterViewInit {

  MAX_SMALL_WIDTH = 520;
  refreshDataSubject$ = new Subject<void>();

  screenWidth = signal(window.innerWidth);
  isSmallScreen = computed(() => this.screenWidth() <= this.MAX_SMALL_WIDTH);
  data = signal<CatalogItemReadModel[]>([]);
  resultsLength = signal(0);
  isLoadingResults = signal(true);
  apiError = signal<CatalogDomainErrorDTO | null>(null);
  selectedCatalogItem = signal<ICatalogItemDTO>({name: ''});
  selectedCatalogItemId = signal<string | null>(null);
  isCatalogItemSaving = signal(false);
  destroyRef$ = inject(DestroyRef);

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth.set(event.target.innerWidth);
  }

  get displayedColumns(): string[] {
    return this.isSmallScreen() ?  ['Name', 'Edit', 'Delete'] : ['Name','Type', 'Brand', 'Edit', 'Delete'];
  }

  constructor(private catalogItemClient: CatalogItemClient,
              private router: Router,
              private route: ActivatedRoute,
              private location: Location) {
    afterNextRender(() => {
      const paramsMap = this.route.snapshot.queryParamMap;
      this.paginator.pageIndex = +(paramsMap.get('pageIndex') ?? '0');
      this.paginator.pageSize = +(paramsMap.get('pageSize') ?? '5');
      this.sort.active =  paramsMap.get('orderBy') ?? '';
      this.sort.direction = paramsMap.get('orderByDirection') as SortDirection ?? '';
    });
  }

  ngAfterViewInit(): void {

    this.sort.sortChange.pipe(takeUntilDestroyed(this.destroyRef$))
      .subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page, this.refreshDataSubject$)
    .pipe(
      takeUntilDestroyed(this.destroyRef$),
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.updateLocation();
        this.isLoadingResults.set(true);
        const orderBy = ListCatalogItemOrderBy[this.sort.active  as keyof typeof ListCatalogItemOrderBy];
        const orderByDirection = OrderByDirections[this.sort.direction.toUpperCase() as keyof typeof OrderByDirections];
        return this.catalogItemClient.getCatalogItems(
          orderBy,
          orderByDirection,
          this.paginator.pageIndex,
          this.paginator.pageSize,
          null, null, null).pipe(catchError(() => observableOf(null)));
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
        return data.catalogItems ?? [];
      }),
    )
    .subscribe(data => this.data.set(data));
  }

  editItem(item: CatalogItemReadModel) {
    this.router.navigate(['edit', item.id], {relativeTo: this.route});
  }

  deleteItem(item: CatalogItemReadModel) {
    this.isLoadingResults.set(true);
    this.catalogItemClient.deleteCatalogItem(item.id!).subscribe({
      error: (e) => {
        this.isLoadingResults.set(false);
        this.apiError.set(e);
      },
      complete: () => {
        this.isCatalogItemSaving.set(false);
        this.refreshDataSubject$.next();
        this.apiError.set(null);
      }
    });
  }

  openCreateItem() {
    this.router.navigate(['new'], {relativeTo: this.route});
  }

  updateLocation() {
    const params =  {
      pageIndex : this.paginator.pageIndex,
      pageSize : this.paginator.pageSize,
      orderBy : this.sort.direction.valueOf() === '' ? null : this.sort.active,
      orderByDirection : this.sort.direction.valueOf() === '' ? null : this.sort.direction
    };

    const urlTree = this.router.createUrlTree([], {
      relativeTo: this.route,
      queryParams: params,
      queryParamsHandling: 'merge',
    });

    this.location.go(urlTree.toString());
  }
}
