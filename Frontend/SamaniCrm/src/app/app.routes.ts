import { Routes } from '@angular/router';
import { TestyComponent } from './testy/testy.component';

export const routes: Routes = [
  //{ path: '', redirectTo: 'testy', pathMatch: 'full' },
  { path: '', redirectTo: 'account', pathMatch: 'full' },
  { path: 'account', loadChildren: () => import('./account/account.module').then((m) => m.AccountModule) },
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then((m) => m.DashboardModule) },

  { path: 'testy', component: TestyComponent },

  //   {
  //     path: 'pagebuilder',
  //     loadChildren: () => import('../page-builder/page-builder.module').then((m) => m.PageBuilderModule),
  //   },

  { path: '**', component: TestyComponent },
];
