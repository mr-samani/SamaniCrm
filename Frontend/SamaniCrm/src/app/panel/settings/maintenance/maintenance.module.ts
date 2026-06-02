import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaintenanceComponent } from './maintenance.component';
import { MaintenanceRoutingModule } from './maintenance-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { TranslateModule } from '@ngx-translate/core';
import { TableViewModule } from '@shared/components/table-view/table-view.module';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { SwitchModule } from '@shared/components/switch/switch.module';
import { CacheComponent } from './tabs/cache/cahce.component';
import { SharedModule } from '@shared/shared.module';
import { DatabaseLogsComponent } from './tabs/database-logs/database-logs.component';
import { MaintenanceServiceProxy } from '@shared/service-proxies/api/maintenance.service';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { AppLogsServiceProxy } from '@shared/service-proxies/api/app-logs.service';
import { SecurityLogsComponent } from './tabs/security-logs/security-logs.component';
import { AutoCompleteTenantComponent } from '@app/Tenants/auto-complete-tenant/auto-complete-tenant.component';
import { SecurityLogServiceProxy } from '@shared/service-proxies/api/security-log.service';

@NgModule({
  declarations: [MaintenanceComponent, CacheComponent, DatabaseLogsComponent, SecurityLogsComponent],
  imports: [
    CommonModule,
    MaintenanceRoutingModule,
    TableViewModule,
    PaginationComponent,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    FileManagerModule,
    MaterialCommonModule,
    TabGroupModule,
    SwitchModule,
    SharedModule,
    AutoCompleteTenantComponent,
  ],
  providers: [MaintenanceServiceProxy, AppLogsServiceProxy, SecurityLogServiceProxy],
})
export class MaintenanceModule {}
