import { NgModule } from '@angular/core';
import { TenantsComponent } from './tenants.component';
import { RouterModule, Routes } from '@angular/router';
import { CreateTenantComponent } from './create-tenant/create-tenant.component';
import { ProvisioningTenantComponent } from './provisioning-tenant/provisioning-tenant.component';

const routes: Routes = [
  { path: '', component: TenantsComponent },
  { path: 'create', component: CreateTenantComponent },
  { path: 'provisioning/:tenantSlug/:tenantId', component: ProvisioningTenantComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TenantsRoutingModule {}
