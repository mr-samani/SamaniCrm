import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SettingsRoutingModule } from './settings-routing.module';
import { SettingsComponent } from './settings.component';
import { SettingMenuService } from './setting.menu.service';
import { SharedModule } from '@shared/shared.module';

@NgModule({
  declarations: [SettingsComponent],
  imports: [CommonModule, SettingsRoutingModule, SharedModule],
  providers: [SettingMenuService],
})
export class SettingsModule {}
