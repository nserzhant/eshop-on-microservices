import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { catchError, take, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const matSnackBar = inject(MatSnackBar);
  const translateService = inject(TranslateService);

  return next(req).pipe(
    catchError(error => {
      if (error instanceof HttpErrorResponse) {
        let errorMessage = '';
        switch (error.status) {
          case 400: // Application generated error should be handled by components/service code
            break;
          case 401:
            errorMessage = 'errors.unauthorized';
            break;
          case 403:
            errorMessage = 'errors.access-is-denied';
            break;
          case 500:
            errorMessage = 'errors.internal-server-error';
            break;
          default:
            errorMessage = 'errors.unknown-error';
        }

        if (errorMessage) {
          translateService.get(errorMessage).pipe(take(1))
            .subscribe(translated => matSnackBar.open(translated, 'Close', { duration: 3000 }));
        }
      }

      return throwError(() => error);
    })
  );
};
