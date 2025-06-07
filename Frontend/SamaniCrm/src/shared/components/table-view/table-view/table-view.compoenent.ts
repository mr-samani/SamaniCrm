import {
  AfterContentInit,
  Component,
  ContentChildren,
  EventEmitter,
  Injector,
  Input,
  Output,
  QueryList,
  TemplateRef,
} from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType, SortEvent } from '../fields-type.model';
import { TableCellDirective } from '../directives/table-cell.directive';

@Component({
  selector: 'table-view',
  templateUrl: './table-view.compoenent.html',
  styleUrls: ['./table-view.compoenent.scss'],
  standalone: false,
})
export class TableViewComponent extends AppComponentBase implements AfterContentInit {
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

  @ContentChildren(TableCellDirective) cellTemplates!: QueryList<TableCellDirective>;
  templateMap: Map<string, TemplateRef<any>> = new Map();

  ngAfterContentInit(): void {
    this.templateMap.clear();
    this.cellTemplates.forEach((cell) => {
      console.log(cell.columnName, cell.template);
      this.templateMap.set(cell.columnName, cell.template);
    });
  }

  // getTemplate(column: string): TemplateRef<any> | null {
  //   return this.templateMap.get(column) ?? null;
  // }
}
