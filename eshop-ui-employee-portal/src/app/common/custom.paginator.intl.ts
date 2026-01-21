import { Injectable } from '@angular/core';
import {MatPaginatorIntl} from '@angular/material/paginator';
import { TranslateParser, TranslateService } from '@ngx-translate/core';
import { Subject, take} from 'rxjs';

@Injectable()
export class CustomPaginatorIntl implements MatPaginatorIntl {
  changes = new Subject<void>();

  firstPageLabel =  '';
  itemsPerPageLabel =  '';
  lastPageLabel =  '';
  nextPageLabel =  '';
  previousPageLabel =  '';
  currentPageLabelTempl = '';

  constructor(private translateService: TranslateService,
              private translateParser: TranslateParser){

    this.translateService.get([
      'paginator.firstPageLabel',
      'paginator.itemsPerPageLabel',
      'paginator.lastPageLabel',
      'paginator.nextPageLabel',
      'paginator.previousPageLabel',
      'paginator.currentPageLabelTemplate'
    ]).pipe(take(1))
    .subscribe( translated => {
      this.firstPageLabel = translated['paginator.firstPageLabel'];
      this.itemsPerPageLabel = translated['paginator.itemsPerPageLabel'];
      this.lastPageLabel = translated['paginator.lastPageLabel'];
      this.nextPageLabel = translated['paginator.nextPageLabel'];
      this.previousPageLabel = translated['paginator.previousPageLabel'];
      this.currentPageLabelTempl = translated['paginator.currentPageLabelTemplate'];
      this.changes.next();
    });
  }

  getRangeLabel(page: number, pageSize: number, length: number): string {
    const startIndex = page * pageSize + 1;
    const endIndex = Math.min(length, (page + 1) * pageSize);

    return this.translateParser.interpolate(this.currentPageLabelTempl, { startIndex : startIndex, endIndex : endIndex, length: length }) ?? '';
  }
}
