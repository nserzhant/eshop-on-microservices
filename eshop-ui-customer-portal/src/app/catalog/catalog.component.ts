import { AfterViewInit, Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatDrawerMode, MatSidenav } from '@angular/material/sidenav';
import { BehaviorSubject,  lastValueFrom,  merge, of as observableOf, Subject, takeUntil } from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap} from 'rxjs/operators';
import { CatalogBrandClient, CatalogBrandReadModel, CatalogItemClient, CatalogItemReadModel, CatalogTypeClient, CatalogTypeReadModel, ListCatalogItemOrderBy, OrderByDirections } from '../services/api/catalog.api.client';
import { OrderingService } from '../services/ordering.service';

@Component({
  selector: 'catalog',
  templateUrl: './catalog.component.html'
})
export class CatalogComponent implements OnInit, AfterViewInit, OnDestroy {
  MAX_WIDTH = 700;

  isSideNavOpened:boolean= false;
  mode: MatDrawerMode = 'side';
  componentDestroyed$ = new Subject<void>();
  screenWidth$ = new BehaviorSubject<number>(window.innerWidth);
  filterChangedSubject$ = new Subject<void>();

  @ViewChild('sidenav') sidenav: MatSidenav | undefined;
  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth$.next(event.target.innerWidth);
  }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  items  = new Array<CatalogItemReadModel>();
  catalogTypes = new Array<CatalogTypeReadModel>();
  catalogBrands = new Array<CatalogBrandReadModel>();

  nameFilter = '';
  brandFilter = '';
  typeFilter = '';

  resultsLength = 0;
  isLoadingResults = true;
  hidePaginator = false;

  constructor(private catalogTypeClient : CatalogTypeClient,
              private catalogBrandClient : CatalogBrandClient,
              private catalogItemClient : CatalogItemClient,
              private orderingSerivce: OrderingService) {}

  ngOnInit(): void {

    this.screenWidth$.asObservable().pipe(takeUntil(this.componentDestroyed$)).subscribe(width => {
       if (width < this.MAX_WIDTH) {
        this.mode = 'over';
        this.isSideNavOpened = false;
      }
      else if (width >  this.MAX_WIDTH) {
        this.mode = 'side';
        this.isSideNavOpened = true;
      }
    });

    this.loadBrands();
    this.loadTypes();
  }

  ngAfterViewInit(): void {
    merge(this.paginator.page, this.filterChangedSubject$)
    .pipe(
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.isLoadingResults = true;
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
        this.isLoadingResults = false;

        if (data === null) {
          return [];
        }

        // Only refresh the result length if there is new data. In case of rate
        // limit errors, we do not want to reset the paginator to zero, as that
        // would prevent users from re-triggering requests.
        this.resultsLength = data.totalCount ?? 0;
        return data.catalogItems ?? [];
      }),
    )
    .subscribe(data => {
      this.hidePaginator = data.length == 0;
      this.items = data
    });
  }

  ngOnDestroy(): void {
    this.componentDestroyed$.next();
  }

  async loadTypes() {
    const items$ = this.catalogTypeClient.getCatalogTypes(OrderByDirections.ASC);
    this.catalogTypes = (await lastValueFrom(items$)).catalogTypes!;
  }

  async loadBrands() {
    const items$ = this.catalogBrandClient.getCatalogBrands(OrderByDirections.ASC);
    this.catalogBrands = (await lastValueFrom(items$)).catalogBrands!;
  }

  onFilterChange() {
    this.paginator.pageIndex = 0;
    this.filterChangedSubject$.next();
  }

  addItemToBasket(item: CatalogItemReadModel) {
    this.orderingSerivce.addItemToBasket(item)
  }
}
