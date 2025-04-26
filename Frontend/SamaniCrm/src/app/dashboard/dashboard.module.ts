import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HeaderComponent } from './layouts/header/header.component';
import { SideMenuComponent } from './layouts/side-menu/side-menu.component';
import { FooterComponent } from './layouts/footer/footer.component';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MenuDynamicComponent } from './layouts/side-menu/menu-dynamic/menu-dynamic.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { BraedCrumbComponent } from './layouts/braed-crumb/braed-crumb.component';

@NgModule({
  declarations: [
    DashboardComponent,
    HeaderComponent,
    SideMenuComponent,
    MenuDynamicComponent,
    FooterComponent,
    BraedCrumbComponent,
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    FormsModule,
    TranslateModule,
    MatMenuModule,
    MatButtonModule,
    FileManagerModule,
  ],
})
export class DashboardModule {}
