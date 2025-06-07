import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableCellDirective } from './directives/table-cell.directive';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '@shared/shared.module';
import { TableViewComponent } from './table-view/table-view.compoenent';

@NgModule({
  declarations: [TableViewComponent, TableCellDirective],
  imports: [CommonModule, TranslateModule, SharedModule],
  exports: [TableViewComponent, TableCellDirective],
})
export class TableViewModule {}
