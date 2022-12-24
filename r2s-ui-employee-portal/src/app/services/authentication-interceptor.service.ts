import { HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpParams, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { mergeMap, Observable, take } from 'rxjs';
import { AuthenticatedUser, AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationInterceptorService implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService) {}


  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.authenticationService.user
      .pipe(take(1), mergeMap(user => this.processRequestWithToken(user, req, next)));
  }

  // Checks if there is an user available in the authorize service
  // and adds users token to the request
  private processRequestWithToken(user: AuthenticatedUser | null, req: HttpRequest<any>, next: HttpHandler) {
    if (!!user ) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${user.token}`
        }
      });
    }

    return next.handle(req);
  }
}
