import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TableViewComponent } from 'src/shared/components/table-view/table-view.compoenent';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { RolesRoutingModule } from './roles-routing.module';
import { RoleListComponent } from './list/list.component';
import { FileManagerModule } from 'src/app/file-manager/file-manager.module';

import { MaterialCommonModule } from '@shared/material/material.common.module';
import { RolePermissionsListComponent } from './role-permissions/role-permissions.component';
import { TreeViewComponent } from '@shared/components/tree-view/tree-view.component';

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
  providers: [],
})
export class RolesModule {}
