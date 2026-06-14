import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PanelComponent } from './panel.component';
import { panelResolver } from './panel.resolver';

const routes: Routes = [
  {
    path: '',
    component: PanelComponent,
    resolve: {
      data: panelResolver,
    },
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
      {
        path: 'dashboard',
        loadComponent: () => import('../dashboard-management/dashboard.component').then((c) => c.DashboardComponent),
      },
      {
        path: 'user-profile',
        loadComponent: () => import('./user-profile/user-profile.component').then((c) => c.UserProfileComponent),
      },

      { path: 'setting', loadChildren: () => import('./settings/settings.module').then((m) => m.SettingsModule) },
      // {
      //   path: 'media',
      //   loadChildren: () => import('../file-manager/file-manager.module').then((m) => m.FileManagerModule),
      // },
      { path: 'languages', loadChildren: () => import('../language-management/languages.module').then((m) => m.LanguagesModule) },

      {
        path: 'tenants',
        loadChildren: () => import('../tenant-management/tenants.module').then((m) => m.TenantsModule),
      },
      {
        path: 'users',
        loadChildren: () => import('../user-management/users/users.module').then((m) => m.UsersModule),
      },
      {
        path: 'roles',
        loadChildren: () => import('../user-management/roles/roles.module').then((m) => m.RolesModule),
      },

      {
        path: 'app-setting',
        loadChildren: () => import('./settings/app-setting/app-setting.module').then((m) => m.AppSettingModule),
      },
      {
        path: 'maintenance',
        loadChildren: () => import('./settings/maintenance/maintenance.module').then((m) => m.MaintenanceModule),
      },
      { path: 'menu', loadChildren: () => import('../menu-management/menu.module').then((m) => m.MenuModule) },

      { path: 'content', loadChildren: () => import('../content-management/content.module').then((m) => m.ContentModule) },
      {
        path: 'products',
        loadChildren: () => import('../product-management/product.module').then((m) => m.ProductModule),
      },
      {
        path: 'notifications',
        loadChildren: () => import('../notification-management/notification.module').then((m) => m.NotificationModule),
      },
      {
        path: 'subscription',
        loadChildren: () => import('../subscription/subscription.module').then((m) => m.SubscriptionModule),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PanelRoutingModule {}
