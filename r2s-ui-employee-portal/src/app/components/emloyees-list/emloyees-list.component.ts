import { Location } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import { ActivatedRoute, Router } from '@angular/router';
import {BehaviorSubject, merge, of as observableOf, Subject} from 'rxjs';
import {catchError, debounceTime, delay, map, startWith, switchMap, timeout} from 'rxjs/operators';
import { ListUserOrderBy, OrderByDirections, UserReadModel, UsersClient } from 'src/app/services/api/users.api.client';

@Component({
  selector: 'app-emloyees-list',
  templateUrl: './emloyees-list.component.html',
  styleUrls: ['./emloyees-list.component.css']
})
export class EmloyeesListComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = ['Email', 'Roles', 'Edit'];
  currentEmailFilter: string | null = null;
  emailFilterSubject = new BehaviorSubject<string | null>(null);
  resultsLength = 0;
  isLoadingResults = true;
  data: UserReadModel[] = [];

  @ViewChild(MatSort) sort!: MatSort;  
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
  }
  
  constructor(
    private usersClient: UsersClient,
    private router: Router,
    private route: ActivatedRoute,
    private location: Location) { 
  }

  ngAfterViewInit() : void {

    setTimeout(() => {
      const paramsMap = this.route.snapshot.queryParamMap;
      this.paginator.pageIndex = +(paramsMap.get('pageIndex') ?? '0');
      this.paginator.pageSize = +(paramsMap.get('pageSize') ?? '5');
      this.sort.active =  paramsMap.get('orderBy') ?? '';
      this.sort.direction = paramsMap.get('orderByDirection') as SortDirection ?? '';
      this.currentEmailFilter = paramsMap.get('emailFilter');
      this.emailFilterSubject.next(this.currentEmailFilter);
    }, 0);

    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page, this.emailFilterSubject)
    .pipe(
      startWith({}),
      debounceTime(0),
      switchMap((val, index) => {
        this.updateLocation();
        this.isLoadingResults = true;
        const orderBy = ListUserOrderBy[this.sort.active  as keyof typeof ListUserOrderBy];
        const orderByDirection = OrderByDirections[this.sort.direction.toUpperCase() as keyof typeof OrderByDirections];
        return this.usersClient.getUsers(
          orderBy,
          orderByDirection,
          this.paginator.pageSize,
          this.paginator.pageIndex,
          this.emailFilterSubject.value
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
        return data.users ?? [];
      }),
    )
    .subscribe(data => (this.data = data));
  }

  openEditUser(user: UserReadModel) {
    this.router.navigate([user.id], {relativeTo: this.route});
  }

  applyEmailFilter(emailFilter : string) {
    if (emailFilter === '') {
      this.currentEmailFilter = null;
    } else {
      this.currentEmailFilter = emailFilter;
    }
    this.emailFilterSubject.next(this.currentEmailFilter);
  }

  getRoles(user: UserReadModel) {
    let roles = user.roles?.map( r =>' ' + r.name).join().trim();

    return roles;
  }

  updateLocation() {
    const params =  {
      pageIndex : this.paginator.pageIndex,
      pageSize : this.paginator.pageSize,
      orderBy : this.sort.direction.valueOf() === '' ? null : this.sort.active,
      orderByDirection : this.sort.direction.valueOf() === '' ? null : this.sort.direction,
      emailFilter : this.emailFilterSubject.value
    };
    
    const urlTree = this.router.createUrlTree([], {
      relativeTo: this.route,
      queryParams: params,
      queryParamsHandling: 'merge',
    });

    this.location.go(urlTree.toString());
  }
}