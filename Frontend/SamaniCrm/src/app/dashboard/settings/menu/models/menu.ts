export class MenuModel {
  children!: MenuModel[];
  id!: string;
  title!: string;
  url?: string;
  icon?: string;
  active?: boolean;
}
