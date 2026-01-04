import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ContentComponent } from './content.component';
import { PagesComponent } from './pages/pages.component';
import { PageTypeEnum } from '@shared/service-proxies';
import { EditPageComponent } from './edit-page/edit-page.component';

const routes: Routes = [
  {
    path: '',
    component: ContentComponent,
    children: [
      { path: '', redirectTo: 'pages/' + PageTypeEnum[PageTypeEnum.HomePage], pathMatch: 'full' },
      { path: 'pages/:type', component: PagesComponent },
    ],
  },
  { path: 'edit/:id', component: EditPageComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ContentRoutingModule {}
