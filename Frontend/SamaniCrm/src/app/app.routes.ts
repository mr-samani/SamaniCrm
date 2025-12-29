import { Routes } from '@angular/router';
import { TestyComponent } from './testy/testy.component';
import { GridGeneratorComponent } from './grid-generator/grid-generator.component';
import { SpacingControlComponent } from './builder/_libs/spacing-control/spacing-control.component';
import { CComponent } from './c/c.component';

export const routes: Routes = [
  //{ path: '', redirectTo: 'testy', pathMatch: 'full' },
  { path: '', redirectTo: 'panel', pathMatch: 'full' },
  { path: 'account', loadChildren: () => import('./account/account.module').then((m) => m.AccountModule) },
  { path: 'panel', loadChildren: () => import('./panel/panel.module').then((m) => m.PanelModule) },

  { path: 'testy', component: TestyComponent },
  { path: 'g', component: GridGeneratorComponent },
  { path: 's', component: SpacingControlComponent },
  { path: 'c', component: CComponent },

  //   {
  //     path: 'pagebuilder',
  //     loadChildren: () => import('../page-builder/page-builder.module').then((m) => m.PageBuilderModule),
  //   },

  { path: '**', component: TestyComponent },
];
