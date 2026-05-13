import { NgModule } from '@angular/core';
import { TenantsComponent } from './Tenants.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [{ path: '', component: TenantsComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TenantsRoutingModule {}
