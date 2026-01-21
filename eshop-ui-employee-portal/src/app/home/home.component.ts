import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    imports: [
      MatCardModule,
      TranslateModule
    ]
})
export class HomeComponent {
}
