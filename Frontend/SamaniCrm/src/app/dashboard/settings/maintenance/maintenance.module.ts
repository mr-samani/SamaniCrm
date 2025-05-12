import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaintenanceComponent } from './maintenance.component';
import { MaintenanceRoutingModule } from './maintenance-routing.module';
import { FormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { TranslateModule } from '@ngx-translate/core';
import { TableViewComponent } from '@shared/components/table-view/table-view.compoenent';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { SwitchModule } from '@shared/components/switch/switch.module';
import { CacheComponent } from './tabs/cache/cahce.component';
import { MaintenanceServiceProxy } from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';

@NgModule({
  declarations: [MaintenanceComponent, CacheComponent],
  imports: [
    CommonModule,
    MaintenanceRoutingModule,
    TableViewComponent,
    TranslateModule,
    FormsModule,
    FileManagerModule,
    MaterialCommonModule,
    TabGroupModule,
    SwitchModule,
    SharedModule,
  ],
  providers: [MaintenanceServiceProxy],
})
export class MaintenanceModule {}
