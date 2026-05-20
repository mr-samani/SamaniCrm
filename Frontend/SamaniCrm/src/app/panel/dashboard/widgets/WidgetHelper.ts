import { DashboardItemDto } from '@shared/service-proxies';
import { Widget } from './widgets';
import { IWidgetDefinition } from './IWidgetDefinition';

export class WidgetHelper {
  public static async loadWidgets(widgets: DashboardItemDto[], definitions: IWidgetDefinition[]): Promise<Widget[]> {
    const list: Widget[] = [];
    for (let item of widgets) {
      const found = definitions.find((x) => x.name == item.componentName);
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
