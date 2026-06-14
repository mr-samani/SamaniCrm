import { Type } from '@angular/core';
import { GridItemConfig } from '@app/grid-generator/grid-generator.component';
import { IWidgetBase } from './IWidgetBase';

export class Widget<DataType = any> {
  id?: string;
  dashboardId!: string;
  componentName?: string;
  component!: Type<IWidgetBase>;
  config!: GridItemConfig;
  data?: DataType = undefined;
  position?: string;
  constructor(data?: Widget<DataType>) {
    if (data) {
      for (let property in data) {
        if (data.hasOwnProperty(property)) (this as any)[property] = (data as any)[property];
      }
    }
    if (this.data && typeof this.data == 'string') {
      this.data = JSON.parse(this.data);
    }
    if (this.position && typeof this.position == 'string') {
      this.config = { ...this.config, ...JSON.parse(this.position) };
    }
    // todo: default width height
    this.config ??= new GridItemConfig(0, 0, 2, 2);
  }
}
