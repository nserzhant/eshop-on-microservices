import { ApplicationConfig, importProvidersFrom, provideZonelessChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateModule } from '@ngx-translate/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { MAT_PROGRESS_SPINNER_DEFAULT_OPTIONS } from '@angular/material/progress-spinner';
import { CustomPaginatorIntl } from './common/custom.paginator.intl';
import { authInterceptor } from './auth/authentication.interceptor';
import { errorInterceptor } from './common/error.interceptor';
import { CatalogBrandClient, CatalogItemClient, CatalogTypeClient, CATALOG_API_URL } from './services/api/catalog.api.client';
import { BasketClient, BASKET_API_URL } from './services/api/basket.api.client';
import { OrderClient, ORDERING_API_URL } from './services/api/ordering.api.client';
import { environment } from '../environments/environment';
import { routes } from './app.routes';

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
      provide: BASKET_API_URL,
      useValue: environment.basketApi
    },
    {
      provide: ORDERING_API_URL,
      useValue: environment.orderingApi
    },
    CatalogBrandClient,
    CatalogTypeClient,
    CatalogItemClient,
    BasketClient,
    OrderClient
  ]
};
