import { LastNotificationsComponent } from './layouts/last-notifications/last-notifications.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HeaderComponent } from './layouts/header/header.component';
import { SideMenuComponent } from './layouts/side-menu/side-menu.component';
import { FooterComponent } from './layouts/footer/footer.component';
import { FormsModule } from '@angular/forms';
import { MenuDynamicComponent } from './layouts/side-menu/menu-dynamic/menu-dynamic.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { BraedCrumbComponent } from './layouts/braed-crumb/braed-crumb.component';
import { PAGINATION_LABELS } from '@shared/components/pagination/pagination.component';
import { PaginationLocalize } from '@shared/localize/pagination';

@NgModule({
  declarations: [DashboardComponent, HeaderComponent, SideMenuComponent, MenuDynamicComponent, FooterComponent, BraedCrumbComponent, LastNotificationsComponent],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    FormsModule,
    TranslateModule,
    MatMenuModule,
    MatButtonModule,
    FileManagerModule,
  ],
  exports: [],
  providers: [
    {
      provide: PAGINATION_LABELS,
      useFactory: (localize: PaginationLocalize) => localize.labels,
      deps: [PaginationLocalize],
    },
  ],
})
export class DashboardModule {}
