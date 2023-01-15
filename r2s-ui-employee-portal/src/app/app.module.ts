import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { ChangeEmailComponent } from './components/change-email/change-email.component';
import { EmloyeesListComponent } from './components/emloyees-list/emloyees-list.component';
import { EmployeeEditComponent } from './components/employee-edit/employee-edit.component';
import { HomeComponent } from './components/home/home.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthenticationInterceptorService } from './services/authentication-interceptor.service';
import { HeaderComponent } from './components/header/header.component';
import { environment } from 'src/environments/environment';
import { EmployeeAccountClient, EMPLOYEES_API_URL, UsersClient } from './services/api/users.api.client';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { ApiErrorsSummaryComponent } from './components/common/api-errors-summary/api-errors-summary.component';
import { HttpErrorInterceptor } from './services/http-error-interceptor.service';
import { LoginCallbackComponent } from './components/login-callback/login-callback.component';
import { MAT_PROGRESS_SPINNER_DEFAULT_OPTIONS } from '@angular/material/progress-spinner';
import { MaterialModule } from 'src/material.module';

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ChangePasswordComponent,
    ChangeEmailComponent,
    EmloyeesListComponent,
    EmployeeEditComponent,
    HomeComponent,
    HeaderComponent,
    ApiErrorsSummaryComponent,
    LoginCallbackComponent
  ],
  imports: [
    MaterialModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,    
    HttpClientModule,
    TranslateModule.forRoot({
        loader: {
            provide: TranslateLoader,
            useFactory: HttpLoaderFactory,
            deps: [HttpClient]
        }
    })
  ],
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
      provide: EMPLOYEES_API_URL,
      useValue: environment.apiRoot
    },
    {
    provide: MAT_PROGRESS_SPINNER_DEFAULT_OPTIONS,
    useValue: {
        diameter: 24, 
      }
    },
    EmployeeAccountClient,
    UsersClient],
  bootstrap: [AppComponent]
})
export class AppModule { }
