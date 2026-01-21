import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from './authentication.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authenticationService = inject(AuthenticationService);
  const router = inject(Router);
  const authenticated = authenticationService.isAuthenticated();

  if (!authenticated) {
    return router.createUrlTree(['']);
  }

  return true;
};
