import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppSettingComponent } from './app-setting.component';

const routes: Routes = [{ path: '', component: AppSettingComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AppSettingRoutingModule {}
