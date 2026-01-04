import { SideNavMenuItem } from '../models/menu-item.model';

export const menus: SideNavMenuItem[] = [
  { name: 'صفحه اصلی', icon: 'fa fa-home', url: '/home' },
  { name: 'پیشخوان', icon: 'fa fa-chart-network', url: '/admin/panel' },
  {
    name: 'Settings',
    permission: 'management',
    icon: 'fa fa-cog',
    children: [
      { name: 'Languages', url: '/panel/languages', icon: 'fa fa-flag' },
      { name: 'Users', url: '/panel/users', icon: 'fa fa-users' },
      { name: 'سازمان\u200cها', url: '/admin/organization' },
    ],
  },
];
