import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { map, Observable } from "rxjs";
import { Roles } from "./api/employee.api.client";
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authenticationService : AuthenticationService,
              private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
    return this.authenticationService.authUser.pipe(map( user => {
      const authenticated = !!user;
      const requiredRoleNames = route.data['roles'] as Array<string>;      
      const requiredRoles = requiredRoleNames?.map(r=> Roles[r as keyof typeof Roles]) || new Array<Roles>();

      const employeeRoles  = authenticated && user.profile && user!.profile['role'];
      let roleNames = new Array<string>();

      if(employeeRoles) {
        if ( typeof employeeRoles ===  'string') {
          roleNames.push(employeeRoles);
        } else if (Array.isArray(employeeRoles)) {
          roleNames = employeeRoles;
        }
      }
      
      const roles = roleNames?.map(r=> Roles[r as keyof typeof Roles]) || new Array<Roles>();


      const rolesAbsent = requiredRoles.filter(role  => roles.indexOf(role) < 0);

      if(!authenticated || rolesAbsent.length > 0) {
        return this.router.createUrlTree(['']);
      }
      
      return true;
    }));
  }
}
