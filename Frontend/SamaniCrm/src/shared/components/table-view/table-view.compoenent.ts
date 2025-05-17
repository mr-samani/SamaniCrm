import { Component, EventEmitter, Injector, Input, Output, TemplateRef } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType, SortEvent } from './fields-type.model';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '@shared/shared.module';

@Component({
  selector: 'table-view',
  templateUrl: './table-view.compoenent.html',
  styleUrls: ['./table-view.compoenent.scss'],
  standalone: true,
  imports: [CommonModule, TranslateModule, SharedModule],
})
export class TableViewComponent extends AppComponentBase {
  [x: string]: any;
  @Input() fields: FieldsType[] = [];
  @Input() operations?: TemplateRef<any>;
  _list: any[] = [];
  @Input() set list(val: any[]) {
    this._list = val ?? [];
  }
  _loading = false;
  @Input() set loading(val: boolean) {
    this._loading = val;
  }

  sortField = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  @Output() sortChange = new EventEmitter<SortEvent>();
  constructor(injector: Injector) {
    super(injector);
  }

  onSortChange(field: string) {
    if (this.sortField == field) {
      this.sortDirection = this.sortDirection == 'asc' ? 'desc' : 'asc';
    }
    this.sortField = field;
    this.sortChange.emit({
      field: this.sortField,
      direction: this.sortDirection,
    });
  }
}
