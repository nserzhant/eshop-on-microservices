import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private matSnackBar: MatSnackBar) { }


  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      return next.handle(req).pipe(
          catchError(error => {
            if (error instanceof HttpErrorResponse) {
              let errorMessage = '';
              switch (error.status) {
                case 400 : //Application generated error should be handled by components/service code
                  break;
                case 401 :
                  errorMessage = 'Unathorized'
                  break;
                case 403 : 
                  errorMessage = 'Access is denied'
                  break;
                case 500 : 
                  errorMessage = 'Internal server error'
                  break;
                default:
                  errorMessage = 'Unknown error'               
            
              }

              if (errorMessage) {
                  this.matSnackBar.open(errorMessage , 'Close', { duration: 3000, panelClass: 'error-snack-bar' });
              }
            }

            return throwError(() => error);
          })
      );
    }
}