import { AfterViewInit, Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject, merge, of as observableOf, Subject} from 'rxjs';
import { Location } from '@angular/common';
import { catchError, debounceTime, map, startWith, switchMap, takeUntil} from 'rxjs/operators';
import { CatalogDomainErrorDTO, CatalogItemClient, CatalogItemReadModel, ICatalogItemDTO, ListCatalogItemOrderBy, OrderByDirections } from '../../services/api/catalog.api.client';

@Component({
  selector: 'app-catalog-items-list',
  templateUrl: './catalog-items-list.component.html'
})
export class CatalogItemsListComponent implements OnInit, OnDestroy, AfterViewInit {

  MAX_SMALL_WIDTH = 520;

  componentDestroyed$ = new Subject<void>();
  screenWidth$ = new BehaviorSubject<number>(window.innerWidth);
  refreshDataSubject$ = new Subject<void>();
  data: CatalogItemReadModel[] = [];
  resultsLength = 0;
  selectedCatalogItem: ICatalogItemDTO | null = null;
  selectedCatalogItemId: string | null = null;
  apiError : CatalogDomainErrorDTO | null = null;
  isCatalogItemSaving = false;
  isLoadingResults = true;
  isSmallScreen = false;

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth$.next(event.target.innerWidth);
  }

  get displayedColumns(): string[] {
    return this.isSmallScreen ?  ['Name', 'Edit', 'Delete'] :  ['Name','Type', 'Brand', 'Edit', 'Delete'];
  }

  constructor(private catalogItemClient: CatalogItemClient,
              private router: Router,
              private route: ActivatedRoute,
              private location: Location) {}

  ngOnInit(): void {
    this.screenWidth$.asObservable().pipe(takeUntil(this.componentDestroyed$)).subscribe(width => {
         if (width < this.MAX_SMALL_WIDTH) {
          this.isSmallScreen = true;
        }
        else if (width >  this.MAX_SMALL_WIDTH) {
          this.isSmallScreen = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.componentDestroyed$.next();
  }

  ngAfterViewInit(): void {

    setTimeout(() => {
      const paramsMap = this.route.snapshot.queryParamMap;
      this.paginator.pageIndex = +(paramsMap.get('pageIndex') ?? '0');
      this.paginator.pageSize = +(paramsMap.get('pageSize') ?? '5');
      this.sort.active =  paramsMap.get('orderBy') ?? '';
      this.sort.direction = paramsMap.get('orderByDirection') as SortDirection ?? '';
    }, 0);

    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page, this.refreshDataSubject$)
    .pipe(
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.updateLocation();
        this.isLoadingResults = true;
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
    .subscribe(data => (this.data = data));
  }

  editItem(item: CatalogItemReadModel) {
    this.router.navigate(['edit', item.id], {relativeTo: this.route});
  }

  deleteItem(item: CatalogItemReadModel) {
    this.isLoadingResults = true;
    this.catalogItemClient.deleteCatalogItem(item.id!).subscribe({
      error: (e) => {
        this.isLoadingResults = false;
        this.apiError = e;
      },
      complete: () => {
        this.isCatalogItemSaving = false;
        this.refreshDataSubject$.next();
        this.apiError = null;
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
