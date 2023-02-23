import { Component, Input } from '@angular/core';
import { CatalogDomainErrorDTO } from '../../services/api/catalog.api.client';

@Component({
  selector: 'catalog-api-errors-summary',
  templateUrl: './catalog-api-errors-summary.component.html'
})
export class CatalogApiErrorsSummaryComponent {

  catalogDomainError: CatalogDomainErrorDTO | null = null;

  @Input()
  set error(catalogDomainError : CatalogDomainErrorDTO | null) {
    this.catalogDomainError = catalogDomainError;
  }
}
