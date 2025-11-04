import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { catchError, Observable, take, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpErrorInterceptor implements HttpInterceptor {

  constructor(private matSnackBar: MatSnackBar,
              private translateService: TranslateService) { }


  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse) {
          let errorMessage = '';
          switch (error.status) {
            case 400 : //Application generated error should be handled by components/service code
              break;
            case 401 :
              errorMessage = 'errors.unauthorized'
              break;
            case 403 :
              errorMessage = 'errors.access-is-denied'
              break;
            case 500 :
              errorMessage = 'errors.internal-server-error'
              break;
            default:
              errorMessage = 'errors.unknown-error'
          }

          if (errorMessage) {
            this.translateService.get(errorMessage).pipe(take(1))
              .subscribe( translated => this.matSnackBar.open(translated , 'Close', { duration: 3000, panelClass: 'error-snack-bar' }));
          }
        }

        return throwError(() => error);
      })
    );
  }
}
