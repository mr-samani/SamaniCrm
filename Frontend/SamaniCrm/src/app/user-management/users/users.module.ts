import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TranslateModule } from '@ngx-translate/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UsersRoutingModule } from './users-routing.module';
import { UserListComponent } from './list/list.component';

import { MaterialCommonModule } from '@shared/material/material.common.module';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { FilterComponent } from '@shared/components/filter/filter.component';
import { TableViewComponent } from '@shared/components/table-view/table-view.compoenent';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { UserServiceProxy } from '@shared/service-proxies/api/user.service';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { RoleServiceProxy, SecuritySettingsServiceProxy } from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';
import { PasswordInputComponent } from '@shared/components/password-input/password-input.component';
import { CreateOrEditUserComponent } from './create-or-edit-user/create-or-edit-user.component';

@NgModule({
  declarations: [UserListComponent, CreateOrEditUserComponent],
  imports: [
    CommonModule,
    UsersRoutingModule,
    TableViewComponent,
    FilterComponent,
    PaginationComponent,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    FileManagerModule,
    MaterialCommonModule,
    TabGroupModule,
    SharedModule,
    PasswordInputComponent,
  ],
  providers: [UserServiceProxy, RoleServiceProxy, SecuritySettingsServiceProxy],
})
export class UsersModule {}
