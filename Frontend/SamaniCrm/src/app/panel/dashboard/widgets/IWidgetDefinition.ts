import { Type } from "@angular/core";

export interface IWidgetDefinition<D = any> {
  title: string;
  name: string;
  component: () => Promise<Type<any>>;
  data?: D;
}
