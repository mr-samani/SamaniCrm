import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TenantsComponent } from './Tenants.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { TranslateModule } from '@ngx-translate/core';
import { FilterComponent } from '@shared/components/filter/filter.component';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { PasswordInputComponent } from '@shared/components/password-input/password-input.component';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { TableViewModule } from '@shared/components/table-view/table-view.module';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { SharedModule } from '@shared/shared.module';
import { TenantsServiceProxy } from '@shared/service-proxies/api/tenants.service';
import { UserServiceProxy } from '@shared/service-proxies/api/user.service';
import { TenantsRoutingModule } from './Tenants-routing.module';

@NgModule({
  declarations: [TenantsComponent],
  imports: [
    CommonModule,
    TenantsRoutingModule,
    TableViewModule,
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
  providers: [TenantsServiceProxy, UserServiceProxy],
})
export class TenantsModule {}
