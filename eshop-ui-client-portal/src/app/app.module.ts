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

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({ declarations: [
        AppComponent,
        HeaderComponent,
        CatalogComponent,
        LoginCallbackComponent
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
        {
            provide: CATALOG_API_URL,
            useValue: environment.catalogApi
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthenticationInterceptorService,
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
