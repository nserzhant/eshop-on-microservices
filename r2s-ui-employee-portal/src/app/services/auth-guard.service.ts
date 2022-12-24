import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { map, Observable } from "rxjs";
import { Roles } from "./api/users.api.client";
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authenticationService : AuthenticationService,
              private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
    return this.authenticationService.user.pipe(map( user => {
      const authenticated = !!user;
      const requiredRoleNames = route.data['roles'] as Array<string>;      
      const requiredRoles = requiredRoleNames?.map(r=> Roles[r as keyof typeof Roles]) || new Array<Roles>();
      const rolesAbsent = requiredRoles.filter(role  => !!user && !!user.roles && user!.roles!.indexOf(role) < 0);

      if(!authenticated || rolesAbsent.length > 0) {
        return this.router.createUrlTree(['/login']);
      }
      
      return true;
    }));
  }
}
