import { Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDrawerMode, MatSidenav } from '@angular/material/sidenav';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'catalog',
  templateUrl: './catalog.component.html'
})
export class CatalogComponent implements OnInit, OnDestroy {
  MAX_WIDTH = 700;

  isSideNavOpened:boolean= false;
  mode: MatDrawerMode = 'side';
  componentDestroyed$ = new Subject<void>();
  screenWidth$ = new BehaviorSubject<number>(window.innerWidth);

  @ViewChild('sidenav') sidenav: MatSidenav | undefined;
  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.screenWidth$.next(event.target.innerWidth);
  }

  items  = new Array<string>();
  types = new Array<{name : string, id: number}>();
  brands = new Array<{name : string, id: number}>();

  constructor() {
    for( let i = 0; i < 24; i++) {
      this.items.push(`item ${i}`);
    }
    for( let i = 0; i < 4; i++) {
      this.types.push({name: `type ${i}`, id: i});
    }
    for( let i = 0; i < 9; i++) {
      this.brands.push({name: `brand ${i}`, id: i});
    }
  }

  ngOnInit(): void {
    this.screenWidth$.asObservable().pipe(takeUntil(this.componentDestroyed$)).subscribe(width => {
       if (width < this.MAX_WIDTH) {
        this.mode = 'over';
        this.isSideNavOpened = false;
      }
      else if (width >  this.MAX_WIDTH) {
        this.mode = 'side';
        this.isSideNavOpened = true;
      }
    });
  }

  ngOnDestroy(): void {
    this.componentDestroyed$.next();
  }
}
