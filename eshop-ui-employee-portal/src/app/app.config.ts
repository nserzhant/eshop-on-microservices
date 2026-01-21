import { ApplicationConfig, importProvidersFrom, provideZonelessChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateModule } from '@ngx-translate/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { CustomPaginatorIntl } from './common/custom.paginator.intl';
import { authInterceptor } from './auth/auth.interceptor';
import { errorInterceptor } from './common/error.interceptor';
import { CatalogBrandClient, CatalogItemClient, CatalogTypeClient, CATALOG_API_URL } from './catalog/services/api/catalog.api.client';
import { EmployeeManagementClient, EMPLOYEE_API_URL } from './employee-management/services/api/employee.api.client';
import { environment } from '../environments/environment';
import { routes } from './app.routes';
import { MAT_PROGRESS_SPINNER_DEFAULT_OPTIONS } from '@angular/material/progress-spinner';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    importProvidersFrom(TranslateModule.forRoot()),
    provideTranslateHttpLoader(),
    {
      provide: MatPaginatorIntl,
      useClass: CustomPaginatorIntl
    },
    {
      provide: MAT_PROGRESS_SPINNER_DEFAULT_OPTIONS,
      useValue: {
        diameter: 24,
      }
    },
    {
      provide: CATALOG_API_URL,
      useValue: environment.catalogApi
    },
    {
      provide: EMPLOYEE_API_URL,
      useValue: environment.apiRoot
    },
    CatalogBrandClient,
    CatalogTypeClient,
    CatalogItemClient,
    EmployeeManagementClient
  ]
};
