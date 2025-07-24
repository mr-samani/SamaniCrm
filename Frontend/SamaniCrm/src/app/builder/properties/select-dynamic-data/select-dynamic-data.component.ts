import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IDataStructure } from '@app/builder/dynamic-data.service';

@Component({
  selector: 'select-dynamic-data',
  templateUrl: './select-dynamic-data.component.html',
  standalone: false,
})
export class SelectDynamicDataComponent {
  @Input() list: IDataStructure[] = [];
  @Output() keys = new EventEmitter<string[]>();

  selectedData?: IDataStructure;

  selectedKeys = '';
  onSelectionChange() {
    if (this.selectedData && this.selectedData.key) {
      this.selectedKeys = this.selectedData.key;
      this.keys.emit([this.selectedKeys]);
    }
  }

  onChangeKey(event: string[]) {
    let array = [this.selectedKeys, ...event];
    this.keys.emit(array);
  }
}
