import { Component } from '@angular/core';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { HeaderComponent } from './header/header.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    imports: [
      RouterModule,
      TranslateModule,
      HeaderComponent
    ]
})
export class AppComponent {
  title = 'eshop-ui-employee-portal';

  constructor(translateService: TranslateService) {
     // this language will be used as a fallback when a translation isn't found in the current language
     translateService.setFallbackLang('en');

     // the lang to use, if the lang isn't available, it will use the current loader to get them
     translateService.use('en');
   }
}
