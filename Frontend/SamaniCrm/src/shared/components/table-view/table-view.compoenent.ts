import { Component, Injector, Input, Output, TemplateRef } from '@angular/core';
import { AppComponentBase } from 'src/app/app-component-base';
import { FieldsType } from './fields-type.model';
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
  constructor(injector: Injector) {
    super(injector);
  }
}
