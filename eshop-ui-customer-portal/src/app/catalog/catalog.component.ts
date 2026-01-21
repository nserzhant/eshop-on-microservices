import { AfterViewInit, Component, computed, HostListener, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatDrawerMode, MatSidenav } from '@angular/material/sidenav';
import { lastValueFrom, merge, of as observableOf, Subject } from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap} from 'rxjs/operators';
import { TranslateModule } from '@ngx-translate/core';
import { CatalogBrandClient, CatalogBrandReadModel, CatalogItemClient, CatalogItemReadModel, CatalogTypeClient, CatalogTypeReadModel, ListCatalogItemOrderBy, OrderByDirections } from '../services/api/catalog.api.client';
import { BasketService } from '../services/basket.service';
import { MATERIAL_TABLE_IMPORTS, MATERIAL_FORM_IMPORTS, MATERIAL_COMMON_IMPORTS } from '../shared/material-imports';

@Component({
    selector: 'catalog',
    templateUrl: './catalog.component.html',
    imports: [
      FormsModule,
      TranslateModule,
      ...MATERIAL_TABLE_IMPORTS,
      ...MATERIAL_FORM_IMPORTS,
      ...MATERIAL_COMMON_IMPORTS
    ]
})
export class CatalogComponent implements OnInit, AfterViewInit, OnDestroy {
  MAX_WIDTH = 700;

  private catalogTypeClient = inject(CatalogTypeClient);
  private catalogBrandClient = inject(CatalogBrandClient);
  private catalogItemClient = inject(CatalogItemClient);
  private orderingSerivce = inject(BasketService);

  isSideNavOpened:boolean= false;
  mode: MatDrawerMode = 'side';
  componentDestroyed$ = new Subject<void>();
  screenWidth = signal(window.innerWidth);
  isSmallScreen = computed(() => this.screenWidth() < this.MAX_WIDTH);
  filterChangedSubject$ = new Subject<void>();

  @ViewChild('sidenav') sidenav: MatSidenav | undefined;
  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth.set(event.target.innerWidth);
  }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  items = signal<CatalogItemReadModel[]>([]);
  catalogTypes = signal<CatalogTypeReadModel[]>([]);
  catalogBrands = signal<CatalogBrandReadModel[]>([]);

  nameFilter = '';
  brandFilter = '';
  typeFilter = '';

  resultsLength = signal(0);
  isLoadingResults = signal(true);
  hidePaginator = signal(false);

  ngOnInit(): void {

    this.screenWidth.set(window.innerWidth);
    if (this.screenWidth() < this.MAX_WIDTH) {
      this.mode = 'over';
      this.isSideNavOpened = false;
    } else {
      this.mode = 'side';
      this.isSideNavOpened = true;
    }

    this.loadBrands();
    this.loadTypes();
  }

  ngAfterViewInit(): void {
    merge(this.paginator.page, this.filterChangedSubject$)
    .pipe(
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.isLoadingResults.set(true);
        const orderBy = ListCatalogItemOrderBy.Name;
        const orderByDirection = OrderByDirections.ASC
        return this.catalogItemClient.getCatalogItems(
          orderBy,
          orderByDirection,
          this.paginator.pageIndex,
          this.paginator.pageSize,
          this.nameFilter,
          this.brandFilter,
          this.typeFilter).pipe(catchError(() => observableOf(null)));
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
    .subscribe(data => {
      this.hidePaginator.set(data.length == 0);
      this.items.set(data);
    });
  }

  ngOnDestroy(): void {
    this.componentDestroyed$.next();
  }

  async loadTypes() {
    const items$ = this.catalogTypeClient.getCatalogTypes(OrderByDirections.ASC);
    this.catalogTypes.set((await lastValueFrom(items$)).catalogTypes!);
  }

  async loadBrands() {
    const items$ = this.catalogBrandClient.getCatalogBrands(OrderByDirections.ASC);
    this.catalogBrands.set((await lastValueFrom(items$)).catalogBrands!);
  }

  onFilterChange() {
    this.paginator.pageIndex = 0;
    this.filterChangedSubject$.next();
  }

  addItemToBasket(item: CatalogItemReadModel) {
    this.orderingSerivce.addItemToBasket(item)
  }
}
