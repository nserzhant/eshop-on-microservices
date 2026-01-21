import { CommonModule, Location } from '@angular/common';
import { AfterViewInit, afterNextRender, Component, computed, HostListener, signal, ViewChild, inject, DestroyRef } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { BehaviorSubject, merge, of as observableOf} from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap} from 'rxjs/operators';
import { EmployeeManagementClient, EmployeeReadModel, ListEmployeeOrderBy, OrderByDirections } from '../services/api/employee.api.client';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MATERIAL_COMPONENTS_IMPORTS } from 'src/app/shared/material-imports';

@Component({
    selector: 'app-employees-list',
    templateUrl: './employees-list.component.html',
    imports: [
      CommonModule,
      RouterModule,
      FormsModule,
      TranslateModule,
      ...MATERIAL_COMPONENTS_IMPORTS
    ]
})
export class EmployeesListComponent implements AfterViewInit {

  MAX_SMALL_WIDTH = 430;

  emailFilterSubject$ = new BehaviorSubject<string | null>(null);

  resultsLength = signal(0);
  isLoadingResults = signal(true);
  data = signal<EmployeeReadModel[]>([]);
  screenWidth = signal(window.innerWidth);
  isSmallScreen = computed(() => this.screenWidth() <= this.MAX_SMALL_WIDTH);

  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth.set(event.target.innerWidth);
  }

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  destroyRef$ = inject(DestroyRef);

  get displayedColumns(): string[] {
    return this.isSmallScreen() ? ['Email', 'Edit'] : ['Email', 'Roles', 'Edit'];
  }

  constructor(private employeeManagementClient: EmployeeManagementClient,
              private router: Router,
              private route: ActivatedRoute,
              private location: Location) {
    afterNextRender(() => {
      const paramsMap = this.route.snapshot.queryParamMap;
      this.paginator.pageIndex = +(paramsMap.get('pageIndex') ?? '0');
      this.paginator.pageSize = +(paramsMap.get('pageSize') ?? '5');
      this.sort.active =  paramsMap.get('orderBy') ?? '';
      this.sort.direction = paramsMap.get('orderByDirection') as SortDirection ?? '';
      this.emailFilterSubject$.next(paramsMap.get('emailFilter'));
    });
  }

  ngAfterViewInit() : void {
    this.sort.sortChange.pipe(takeUntilDestroyed(this.destroyRef$))
      .subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page, this.emailFilterSubject$)
    .pipe(
      takeUntilDestroyed(this.destroyRef$),
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.updateLocation();
        this.isLoadingResults.set(true);
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
        this.isLoadingResults.set(false);

        if (data === null) {
          return [];
        }

        // Only refresh the result length if there is new data. In case of rate
        // limit errors, we do not want to reset the paginator to zero, as that
        // would prevent users from re-triggering requests.
        this.resultsLength.set(data.totalCount ?? 0);
        return data.employees ?? [];
      }),
    )
    .subscribe(data => this.data.set(data));
  }

  openEditUser(employee: EmployeeReadModel) {
    this.router.navigate([employee.id], {relativeTo: this.route});
  }

  applyEmailFilter(emailFilter : string) {
    this.emailFilterSubject$.next(emailFilter === '' ? null : emailFilter);
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
