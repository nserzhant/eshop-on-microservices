import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { HttpErrorInterceptor } from './services/http-error-interceptor.service';
import { MAT_PROGRESS_SPINNER_DEFAULT_OPTIONS } from '@angular/material/progress-spinner';
import { MaterialModule } from 'src/material.module';
import { CatalogModule } from './catalog/catalog.module';
import { HomeComponent } from './home/home.component';
import { HeaderComponent } from './header/header.component';
import { LoginCallbackComponent } from './auth/login-callback.component';
import { AuthenticationInterceptorService } from './auth/authentication-interceptor.service';
import { EmployeeManagementModule } from './employee-management/employee-management.module';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { CustomPaginatorIntl } from './common/custom.paginator.intl';

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
      AppComponent,
      HomeComponent,
      HeaderComponent,
      LoginCallbackComponent
  ],
  bootstrap: [AppComponent],
  imports: [
    MaterialModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    CatalogModule,
    EmployeeManagementModule,
    TranslateModule.forRoot({
        loader: {
            provide: TranslateLoader,
            useFactory: HttpLoaderFactory,
            deps: [HttpClient]
        }
    })],
  providers: [
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
    provideHttpClient(withInterceptorsFromDi()),
    { provide: MatPaginatorIntl, useClass: CustomPaginatorIntl }
  ]
})
export class AppModule { }
