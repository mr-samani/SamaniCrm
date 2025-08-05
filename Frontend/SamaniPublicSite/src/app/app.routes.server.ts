import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'home',
    renderMode: RenderMode.Client,
  },
  {
    path: 'page-preview-old/:culture/:id',
    renderMode: RenderMode.Client,
  },
  {
    path: 'page/preview/:culture/:pageId',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'page/:culture/:slug',
    renderMode: RenderMode.Prerender,
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender,
  },
];
