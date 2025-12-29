import { LastNotificationsComponent } from './layouts/last-notifications/last-notifications.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PanelRoutingModule } from './panel-routing.module';
import { PanelComponent } from './panel.component';
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
import { SharedModule } from '@shared/shared.module';
import { NotificationServiceProxy } from '@shared/service-proxies';

@NgModule({
  declarations: [
    PanelComponent,
    HeaderComponent,
    SideMenuComponent,
    MenuDynamicComponent,
    FooterComponent,
    BraedCrumbComponent,
    LastNotificationsComponent,
  ],
  imports: [
    CommonModule,
    PanelRoutingModule,
    FormsModule,
    TranslateModule,
    MatMenuModule,
    MatButtonModule,
    FileManagerModule,
    SharedModule,
  ],
  exports: [],
  providers: [
    {
      provide: PAGINATION_LABELS,
      useFactory: (localize: PaginationLocalize) => localize.labels,
      deps: [PaginationLocalize],
    },
    NotificationServiceProxy,
  ],
})
export class PanelModule {}
