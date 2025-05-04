import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LanguageListComponent } from './language-list/language-list.component';

const routes: Routes = [
  { path: '', component: LanguageListComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LanguagesRoutingModule {}
