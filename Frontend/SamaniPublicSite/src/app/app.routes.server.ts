import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: ':culture/home',
    renderMode: RenderMode.Client,
  },
  {
    path: ':culture/page/preview/:culture/:pageId',
    renderMode: RenderMode.Client,
  },
  {
    path: ':culture/page/:culture/:slug',
    renderMode: RenderMode.Prerender,
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender,
  },
];
