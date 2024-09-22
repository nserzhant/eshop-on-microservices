import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { map, Observable, switchMap } from "rxjs";
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard  {

  constructor(private authenticationService : AuthenticationService,
              private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {

    return this.authenticationService.initCompleted$.pipe(
      switchMap((_)=> {
        return this.authenticationService.authUser$.pipe(map( user => {
          const authenticated = !!user;
          const requiredRoleNames = (route.data['roles'] ?? new Array<string>()) as Array<string>;

          const employeeRoles  = authenticated && user.profile && (user!.profile['role'] ?? user!.profile['roles'] );
          let roleNames = new Array<string>();

          if(employeeRoles) {
            if ( typeof employeeRoles ===  'string') {
              roleNames.push(employeeRoles);
            } else if (Array.isArray(employeeRoles)) {
              roleNames = employeeRoles;
            }
          }

          const rolesAbsent =  requiredRoleNames.filter(roleName => roleNames.indexOf(roleName) < 0 );

          if(!authenticated || rolesAbsent.length > 0) {
            return this.router.createUrlTree(['']);
          }

          return true;
        }));
      })
    );
  }
}
