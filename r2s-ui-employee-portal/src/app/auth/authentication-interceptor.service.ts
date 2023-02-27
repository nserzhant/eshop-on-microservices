import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from 'oidc-client-ts';
import { mergeMap, Observable, take, from } from 'rxjs';
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationInterceptorService implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService) {}


  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this.authenticationService.userManager.getUser())
      .pipe(take(1), mergeMap(user => this.processRequestWithAccessToken(user, req, next)));
  }

  // Checks if there is an user available in the authorize service
  // and adds users token to the request
  private processRequestWithAccessToken(user: User | null, req: HttpRequest<any>, next: HttpHandler) {
    if (!!user ) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${user.access_token}`
        }
      });
    }

    return next.handle(req);
  }
}
