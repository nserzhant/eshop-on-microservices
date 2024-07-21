import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HeaderComponent } from './header/header.component';
import { MaterialModule } from 'src/material.module';
import { HttpClient, HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { LoginCallbackComponent } from './auth/login-callback.component';
import { CatalogComponent } from './catalog/catalog.component';
import { CatalogBrandClient, CatalogItemClient, CatalogTypeClient, CATALOG_API_URL } from './services/api/catalog.api.client';
import { environment } from 'src/environments/environment';
import { AuthenticationInterceptorService } from './auth/authentication-interceptor.service';
import { MAT_PROGRESS_SPINNER_DEFAULT_OPTIONS } from '@angular/material/progress-spinner';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BasketComponent } from './basket/basket.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { OrdersComponent } from './orders/orders.component';
import { BASKET_API_URL, BasketClient } from './services/api/basket.api.client';
import { OrderClient, ORDERING_API_URL } from './services/api/ordering.api.client';
import { HttpErrorInterceptor } from './services/http-error-interceptor.service';

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({ declarations: [
        AppComponent,
        HeaderComponent,
        CatalogComponent,
        LoginCallbackComponent,
        BasketComponent,
        CheckoutComponent,
        OrdersComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        MaterialModule,
        FormsModule,
        ReactiveFormsModule,
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [HttpClient]
            }
        })], providers: [
        CatalogBrandClient,
        CatalogTypeClient,
        CatalogItemClient,
        BasketClient,
        OrderClient,
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
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthenticationInterceptorService,
            multi: true
        },
        {
          provide: HTTP_INTERCEPTORS,
          useClass: HttpErrorInterceptor,
          multi: true
        },
        {
            provide: MAT_PROGRESS_SPINNER_DEFAULT_OPTIONS,
            useValue: {
                diameter: 24,
            }
        },
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule { }
