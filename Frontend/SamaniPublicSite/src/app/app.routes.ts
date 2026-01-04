import { Routes } from '@angular/router';
import { AppConst } from '@shared/app-const';

export const routes: Routes = [
  { path: '', redirectTo: 'fa/home', pathMatch: 'full' },

  {
    path: ':culture',
    loadComponent: () => import('./root-culture/root-culture.component').then((c) => c.RootCultureComponent),
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', loadComponent: () => import('./home/home.component').then((c) => c.HomeComponent) },
      {
        path: 'page-preview',
        loadChildren: () => import('./page-view/page-view.module').then((m) => m.PageViewModule),
      },
      /** New Page Builder */
      {
        path: 'page',
        loadChildren: () => import('./page-view/page-view.module').then((m) => m.PageViewModule),
      },

      /** preview of greapeJS */
      {
        path: 'page-preview-old/:culture/:id',
        loadComponent: () => import('./page-preview-old/page-preview.component').then((c) => c.PagePreviewOldComponent),
      },
    ],
  },

  {
    path: '**',
    loadComponent: () => import('./not-fount/not-fount.component').then((c) => c.NotFountComponent),
  },
];
