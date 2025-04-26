import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoleListComponent } from './list/list.component';
import { RolePermissionsListComponent } from './role-permissions/role-permissions.component';

const routes: Routes = [
  { path: '', component: RoleListComponent },
  { path: 'permissions/:roleName', component: RolePermissionsListComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RolesRoutingModule {}
