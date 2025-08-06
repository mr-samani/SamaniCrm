import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IDataStructure } from '@app/builder/services/dynamic-data.service';

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
      this.selectedNameSpace = parts.shift() ?? '';
      this.childNameSpace = parts.length > 0 ? `{{${parts.join('.')}}}` : '';
      if (this.selectedNameSpace) {
        this.selectedData = {
          nameSpace: this.selectedNameSpace,
          type: 'object',
          children: this.list.find((x) => x.nameSpace == this.selectedNameSpace)?.children ?? [],
        };
      }
    } else {
      this.selectedData = undefined;
    }
  }
  @Output() keys = new EventEmitter<string[]>();

  selectedNameSpace = '';
  onSelectionChange() {
    if (this.selectedData && this.selectedData.nameSpace) {
      this.selectedNameSpace = this.selectedData.nameSpace;
      this.keys.emit([this.selectedNameSpace]);
    }
  }

  onChangeKey(event: string[]) {
    let array = [this.selectedNameSpace, ...event];
    this.keys.emit(array);
  }

  compareFn(a: IDataStructure, b: IDataStructure) {
    return a && b && a.nameSpace == b.nameSpace;
  }
}
