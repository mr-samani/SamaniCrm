import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { dashboardResolver } from './dashboard.resolver';

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent,
    resolve: {
      data: dashboardResolver,
    },
    children: [
      { path: 'setting', loadChildren: () => import('./settings/settings.module').then((m) => m.SettingsModule) },
      {
        path: 'media',
        loadChildren: () => import('../file-manager/file-manager.module').then((m) => m.FileManagerModule),
      },
      { path: 'languages', loadChildren: () => import('./languages/languages.module').then((m) => m.LanguagesModule) },
      {
        path: 'users',
        loadChildren: () => import('..//user-management/users/users.module').then((m) => m.UsersModule),
      },
      {
        path: 'roles',
        loadChildren: () => import('..//user-management/roles/roles.module').then((m) => m.RolesModule),
      },
      {
        path: 'app-setting',
        loadChildren: () => import('./settings/app-setting/app-setting.module').then((m) => m.AppSettingModule),
      },
      {
        path: 'maintenance',
        loadChildren: () => import('./settings/maintenance/maintenance.module').then((m) => m.MaintenanceModule),
      },
      { path: 'menu', loadChildren: () => import('./settings/menu/menu.module').then((m) => m.MenuModule) },

      // products
      {
        path: 'product',
        children: [
          {
            path: 'categories',
            loadComponent: () =>
              import('../products/product-categories/product-categories.component').then(
                (c) => c.ProductCategoriesComponent,
              ),
          },
        ],
      },

      // content
      { path: 'content', loadChildren: () => import('./content/content.module').then((m) => m.ContentModule) },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DashboardRoutingModule {}
