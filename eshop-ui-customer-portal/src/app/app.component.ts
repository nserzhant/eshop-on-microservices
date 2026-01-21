import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { HeaderComponent } from './header/header.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    imports: [RouterOutlet, HeaderComponent]
})
export class AppComponent {
  title = 'eshop-ui-customer-portal';

  constructor(translateService: TranslateService) {
    // this language will be used as a fallback when a translation isn't found in the current language
    translateService.setFallbackLang('en');

    // the lang to use, if the lang isn't available, it will use the current loader to get them
    translateService.use('en');
  }
}
