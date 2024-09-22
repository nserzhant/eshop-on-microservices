import { Location } from '@angular/common';
import { AfterViewInit, Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject, merge, of as observableOf, Subject} from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap, takeUntil} from 'rxjs/operators';
import { EmployeeManagementClient, EmployeeReadModel, ListEmployeeOrderBy, OrderByDirections } from '../services/api/employee.api.client';

@Component({
  selector: 'app-emloyees-list',
  templateUrl: './emloyees-list.component.html'
})
export class EmloyeesListComponent implements OnInit, OnDestroy, AfterViewInit {

  MAX_SMALL_WIDTH = 430;

  componentDestroyed$ = new Subject<void>();
  currentEmailFilter: string | null = null;
  emailFilterSubject$ = new BehaviorSubject<string | null>(null);
  resultsLength = 0;
  isLoadingResults = true;
  data: EmployeeReadModel[] = [];
  isSmallScreen = false;
  screenWidth$ = new BehaviorSubject<number>(window.innerWidth);

  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth$.next(event.target.innerWidth);
  }
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  get displayedColumns(): string[] {
    return this.isSmallScreen ? ['Email', 'Edit'] : ['Email', 'Roles', 'Edit'];
  }

  constructor(private employeeManagementClient: EmployeeManagementClient,
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

  ngAfterViewInit() : void {

    setTimeout(() => {
      const paramsMap = this.route.snapshot.queryParamMap;
      this.paginator.pageIndex = +(paramsMap.get('pageIndex') ?? '0');
      this.paginator.pageSize = +(paramsMap.get('pageSize') ?? '5');
      this.sort.active =  paramsMap.get('orderBy') ?? '';
      this.sort.direction = paramsMap.get('orderByDirection') as SortDirection ?? '';
      this.currentEmailFilter = paramsMap.get('emailFilter');
      this.emailFilterSubject$.next(this.currentEmailFilter);
    }, 0);

    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page, this.emailFilterSubject$)
    .pipe(
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.updateLocation();
        this.isLoadingResults = true;
        const orderBy = ListEmployeeOrderBy[this.sort.active  as keyof typeof ListEmployeeOrderBy];
        const orderByDirection = OrderByDirections[this.sort.direction.toUpperCase() as keyof typeof OrderByDirections];
        return this.employeeManagementClient.getEmployees(
          orderBy,
          orderByDirection,
          this.paginator.pageSize,
          this.paginator.pageIndex,
          this.emailFilterSubject$.value
        ).pipe(catchError(() => observableOf(null)));
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
        return data.employees ?? [];
      }),
    )
    .subscribe(data => (this.data = data));
  }

  openEditUser(employee: EmployeeReadModel) {
    this.router.navigate([employee.id], {relativeTo: this.route});
  }

  applyEmailFilter(emailFilter : string) {
    if (emailFilter === '') {
      this.currentEmailFilter = null;
    } else {
      this.currentEmailFilter = emailFilter;
    }
    this.emailFilterSubject$.next(this.currentEmailFilter);
  }

  getRoles(employee: EmployeeReadModel) {
    const roles = employee.roles?.map( r =>' ' + r.name).join().trim();

    return roles;
  }

  updateLocation() {
    const params =  {
      pageIndex : this.paginator.pageIndex,
      pageSize : this.paginator.pageSize,
      orderBy : this.sort.direction.valueOf() === '' ? null : this.sort.active,
      orderByDirection : this.sort.direction.valueOf() === '' ? null : this.sort.direction,
      emailFilter : this.emailFilterSubject$.value
    };

    const urlTree = this.router.createUrlTree([], {
      relativeTo: this.route,
      queryParams: params,
      queryParamsHandling: 'merge',
    });

    this.location.go(urlTree.toString());
  }
}
