import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppSettingComponent } from './app-setting.component';
import { AppSettingRoutingModule } from './app-setting-routing.module';
import { FormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { TranslateModule } from '@ngx-translate/core';
import { TableViewModule } from '@shared/components/table-view/table-view.module';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { GeneralSettingComponent } from './tabs/general-setting/general-setting.component';
import { SecuritySettingComponent } from './tabs/security-setting/security-setting.component';
import { SwitchModule } from '@shared/components/switch/switch.module';
import { SecuritySettingsServiceProxy } from '@shared/service-proxies';

@NgModule({
  declarations: [AppSettingComponent, GeneralSettingComponent, SecuritySettingComponent],
  imports: [
    CommonModule,
    AppSettingRoutingModule,
    TableViewModule,
    TranslateModule,
    FormsModule,
    FileManagerModule,
    MaterialCommonModule,
    TabGroupModule,
    SwitchModule,
  ],
  providers: [SecuritySettingsServiceProxy],
})
export class AppSettingModule {}
