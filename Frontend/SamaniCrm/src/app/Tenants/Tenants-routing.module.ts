import { NgModule } from '@angular/core';
import { TenantsComponent } from './Tenants.component';
import { RouterModule, Routes } from '@angular/router';
import { CreateTenantComponent } from './create-tenant/create-tenant.component';

const routes: Routes = [
  { path: '', component: TenantsComponent },
  { path: 'create', component: CreateTenantComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TenantsRoutingModule {}
