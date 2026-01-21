import { Component, input } from '@angular/core';
import { MatInputModule } from "@angular/material/input";
import { TranslateModule } from '@ngx-translate/core';
import { CatalogDomainErrorDTO } from '../../services/api/catalog.api.client';

@Component({
    selector: 'catalog-api-errors-summary',
    templateUrl: './catalog-api-errors-summary.component.html',
    imports: [
    TranslateModule,
    MatInputModule
]
})
export class CatalogApiErrorsSummaryComponent {
  error = input<CatalogDomainErrorDTO | null>(null);
}
