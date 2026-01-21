import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatExpansionModule } from '@angular/material/expansion';

export const MATERIAL_TABLE_IMPORTS = [
  MatTableModule,
  MatPaginatorModule
];

export const MATERIAL_FORM_IMPORTS = [
  MatInputModule,
  MatButtonModule,
  MatIconModule,
  MatSnackBarModule,
  MatSelectModule
];

export const MATERIAL_COMMON_IMPORTS = [
  MatExpansionModule,
  MatProgressSpinnerModule,
  MatSidenavModule,
  MatCardModule
];
