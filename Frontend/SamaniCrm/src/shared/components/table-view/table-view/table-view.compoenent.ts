import {
  AfterContentInit,
  Component,
  ContentChildren,
  EventEmitter,
  Injector,
  Input,
  OnInit,
  Output,
  QueryList,
  TemplateRef,
} from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType, SortEvent } from '../fields-type.model';
import { TableCellDirective } from '../directives/table-cell.directive';
import { cloneDeep, orderBy } from 'lodash-es';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'table-view',
  templateUrl: './table-view.compoenent.html',
  styleUrls: ['./table-view.compoenent.scss'],
  standalone: false,
})
export class TableViewComponent extends AppComponentBase implements OnInit, AfterContentInit {
  [x: string]: any;
  @Input() fields: FieldsType[] = [];
  @Input() operations?: TemplateRef<any>;

  private _lazy: boolean = false;
  /**
   * lazy load
   *  @example true  // call with http client
   *  @example false // sort and filter in local
   *
   */
  @Input('lazy') set setLazy(val: boolean) {
    this._lazy = val === true;
  }
  get lazy() {
    return this._lazy;
  }
  private _list: any[] = [];
  private _allList: any[] = [];
  @Input() set list(val: any[]) {
    this._allList = cloneDeep(val ?? []);
    this._list = val ?? [];
    this.totalCount = this._list.length;
    if (this.lazy == false) {
      this.onPageChange({ page: this.page, perPage: this.perPage });
    }
  }
  get list() {
    return this._list;
  }
  _loading = false;
  @Input() set loading(val: boolean) {
    this._loading = val;
  }

  sortField = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  @Output() sortChange = new EventEmitter<SortEvent>();
  @ContentChildren(TableCellDirective) cellTemplates!: QueryList<TableCellDirective>;
  templateMap: Map<string, TemplateRef<any>> = new Map();

  perPage = AppConst.defaultTablePerPage;
  page = 1;
  totalCount = 0;
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
  ngAfterContentInit(): void {
    this.templateMap.clear();
    this.cellTemplates.forEach((cell) => {
      console.log(cell.columnName, cell.template);
      this.templateMap.set(cell.columnName, cell.template);
    });
  }

  onSortChange(field: string) {
    if (this.sortField == field) {
      this.sortDirection = this.sortDirection == 'asc' ? 'desc' : 'asc';
    }
    this.sortField = field;
    if (this.lazy == false) {
      this.sortList();
    }
    this.sortChange.emit({
      field: this.sortField,
      direction: this.sortDirection,
    });
  }

  // getTemplate(column: string): TemplateRef<any> | null {
  //   return this.templateMap.get(column) ?? null;
  // }

  sortList() {
    this._list = orderBy(this._list, [this.sortField], [this.sortDirection]);
  }

  onPageChange(ev: PageEvent) {
    this.perPage = ev.perPage;
    const f = (ev.page - 1) * ev.perPage;
    const t = f + ev.perPage;
    this._list = this._allList.slice(f, t);
  }
}
