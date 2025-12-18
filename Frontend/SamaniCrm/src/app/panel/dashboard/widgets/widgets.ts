import { Type } from '@angular/core';
import { GridItemConfig } from '@app/grid-generator/grid-generator.component';
import { DashboardItemDto } from '@shared/service-proxies/model/dashboard-item-dto';
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
export interface IWidgetDefinition<D = any> {
  title: string;
  name: string;
  component: () => Promise<Type<any>>;
  data?: D;
}
export class WidgetHelper {
  public static WidgetDefinition: IWidgetDefinition[] = [
    {
      title: 'Note',
      name: 'NoteWidget',
      component: () => import('./note/note.component').then((w) => w.NoteComponent),
      data: {
        title: 'This is a text!',
      },
    },
  ];

  public static async loadWidgets(widgets: DashboardItemDto[]): Promise<Widget[]> {
    const list: Widget[] = [];
    for (let item of widgets) {
      const found = WidgetHelper.WidgetDefinition.find((x) => x.name == item.componentName);
      if (found) {
        let w = new Widget(Object.assign(item as any, { component: await found.component() }));
        list.push(w);
      } else {
        console.warn('Widget not found: ', item.componentName);
      }
    }
    return list;
  }
}
