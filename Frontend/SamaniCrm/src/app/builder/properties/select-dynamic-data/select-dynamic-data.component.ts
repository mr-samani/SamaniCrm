import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IDataStructure } from '@app/builder/dynamic-data.service';

@Component({
  selector: 'select-dynamic-data',
  templateUrl: './select-dynamic-data.component.html',
  standalone: false,
})
export class SelectDynamicDataComponent {
  @Input() list: IDataStructure[] = [];
  childNameSpace = '';
  selectedData?: IDataStructure;
  @Input() set value(value: string | undefined) {
    value = typeof value == 'string' ? value.trim() : '';
    if (value.startsWith('{{') && value.endsWith('}}')) {
      const key = value.slice(2, -2);
      const parts = key.split('.');
      const nameSpace = parts.shift() ?? '';
      this.childNameSpace = parts.length > 0 ? `{{${parts.join('.')}}}` : '';
      if (nameSpace) {
        this.selectedData = {
          nameSpace,
          type: 'object',
          children: this.list.find((x) => x.nameSpace == nameSpace)?.children ?? [],
        };
      }
    }
  }
  @Output() keys = new EventEmitter<string[]>();

  selectedKeys = '';
  onSelectionChange() {
    if (this.selectedData && this.selectedData.nameSpace) {
      this.selectedKeys = this.selectedData.nameSpace;
      this.keys.emit([this.selectedKeys]);
    }
  }

  onChangeKey(event: string[]) {
    let array = [this.selectedKeys, ...event];
    this.keys.emit(array);
  }

  compareFn(a: IDataStructure, b: IDataStructure) {
    return a && b && a.nameSpace == b.nameSpace;
  }
}
