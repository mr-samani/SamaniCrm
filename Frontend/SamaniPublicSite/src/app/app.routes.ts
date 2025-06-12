import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', loadComponent: () => import('./home/home.component').then((c) => c.HomeComponent) },
  {
    path: 'page-preview/:culture/:id',
    loadComponent: () => import('./page-preview/page-preview.component').then((c) => c.PagePreviewComponent),
  },
  {
    path: '**',
    loadComponent: () => import('./not-fount/not-fount.component').then((c) => c.NotFountComponent),
  },
];
