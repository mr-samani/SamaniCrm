import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { SwitchModule } from '@shared/components/switch/switch.module';
import { MenuComponent } from './menu.component';
import { MenuRoutingModule } from './menu-routing.module';
import { TreeMenuComponent } from './tree-menu/tree-menu.component';
import { CreateOrEditMenuComponent } from './create-or-edit/create-or-edit.component';
import { MenuServiceProxy } from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';

@NgModule({
  declarations: [MenuComponent, TreeMenuComponent, CreateOrEditMenuComponent],
  imports: [
    CommonModule,
    MenuRoutingModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialCommonModule,
    TabGroupModule,
    SwitchModule,
    SharedModule,
  ],
  providers: [MenuServiceProxy],
})
export class MenuModule {}
