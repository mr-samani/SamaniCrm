import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableCellDirective } from './directives/table-cell.directive';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '@shared/shared.module';
import { TableViewComponent } from './table-view/table-view.compoenent';
import { PaginationComponent } from "@shared/components/pagination/pagination.component";

@NgModule({
  declarations: [TableViewComponent, TableCellDirective],
  imports: [CommonModule, TranslateModule, SharedModule, PaginationComponent],
  exports: [TableViewComponent, TableCellDirective],
})
export class TableViewModule {}
