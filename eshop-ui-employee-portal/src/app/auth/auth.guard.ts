import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthenticationService } from './authentication.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authenticationService = inject(AuthenticationService);
  const router = inject(Router);
  const authenticated = authenticationService.isAuthenticated();
  const requiredRoleNames = (route.data['roles'] ?? []) as string[];
  const employeeRoles = authenticated && authenticationService.userRoles();
  let roleNames: string[] = [];

  if (employeeRoles) {
    if (typeof employeeRoles === 'string') {
      roleNames.push(employeeRoles);
    } else if (Array.isArray(employeeRoles)) {
      roleNames = employeeRoles;
    }
  }

  const rolesAbsent = requiredRoleNames.filter(roleName => roleNames.indexOf(roleName) < 0);

  if (!authenticated || rolesAbsent.length > 0) {
    return router.createUrlTree(['']);
  }

  return true;
};
