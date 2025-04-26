import { SideNavMenuItem } from '../models/menu-item.model';

export const menus: SideNavMenuItem[] = [
  { name: 'صفحه اصلی', icon: 'fa fa-home', url: '/home' },
  { name: 'پیشخوان', icon: 'fa fa-chart-network', url: '/admin/dashboard' },
  {
    name: 'Settings',
    permission: 'management',
    icon: 'fa fa-cog',
    children: [
      { name: 'Languages', url: '/dashboard/languages', icon: 'fa fa-flag' },
      { name: 'Users', url: '/dashboard/users', icon: 'fa fa-users' },
      { name: 'سازمان\u200cها', url: '/admin/organization' },
    ],
  },
];
