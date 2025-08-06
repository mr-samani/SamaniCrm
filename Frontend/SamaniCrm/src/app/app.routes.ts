import { Routes } from '@angular/router';
import { TestyComponent } from './testy/testy.component';
import { GridGeneratorComponent } from './grid-generator/grid-generator.component';
import { SpacingControlComponent } from './builder/_libs/spacing-control/spacing-control.component';

export const routes: Routes = [
  //{ path: '', redirectTo: 'testy', pathMatch: 'full' },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'account', loadChildren: () => import('./account/account.module').then((m) => m.AccountModule) },
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then((m) => m.DashboardModule) },

  { path: 'testy', component: TestyComponent },
  { path: 'g', component: GridGeneratorComponent },
  { path: 's', component: SpacingControlComponent },

  //   {
  //     path: 'pagebuilder',
  //     loadChildren: () => import('../page-builder/page-builder.module').then((m) => m.PageBuilderModule),
  //   },

  { path: '**', component: TestyComponent },
];
