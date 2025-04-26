export class SideNavMenuItem {
  id?: number | string;
  name!: string;
  url?: string;
  icon?: string;
  children?: SideNavMenuItem[];
  isOpen?: boolean;
  permission?: string;
}
