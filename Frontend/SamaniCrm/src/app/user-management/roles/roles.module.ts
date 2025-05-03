import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { RolesRoutingModule } from './roles-routing.module';
import { RoleListComponent } from './list/list.component';

import { MaterialCommonModule } from '@shared/material/material.common.module';
import { RolePermissionsListComponent } from './role-permissions/role-permissions.component';
import { TreeViewComponent } from '@shared/components/tree-view/tree-view.component';
import { TableViewComponent } from '@shared/components/table-view/table-view.compoenent';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { RoleServiceProxy } from '@shared/service-proxies';

@NgModule({
  declarations: [RoleListComponent, RolePermissionsListComponent],
  imports: [
    CommonModule,
    RolesRoutingModule,
    TableViewComponent,
    TranslateModule,
    FormsModule,
    FileManagerModule,
    MaterialCommonModule,
    TreeViewComponent,
  ],
  providers: [RoleServiceProxy],
})
export class RolesModule {}
