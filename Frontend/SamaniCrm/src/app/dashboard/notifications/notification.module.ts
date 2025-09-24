import { NotificationListComponent } from './notification-list/notification-list.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationRoutingModule } from './notification-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { TableViewModule } from '@shared/components/table-view/table-view.module';
import { TranslateModule } from '@ngx-translate/core';
import { NotificationServiceProxy } from '@shared/service-proxies';
import { FilterComponent } from '@shared/components/filter/filter.component';
import { SharedModule } from '@shared/shared.module';
import { SendNotificationDialogComponent } from './send-notification/send-notification.component';
import { AutoCompleteUserComponent } from '@app/user-management/auto-complete-user/auto-complete-user.component';
import { BroadcastNotificationComponent } from './broadcast-notification/broadcast-notification.component';

@NgModule({
  declarations: [NotificationListComponent, SendNotificationDialogComponent, BroadcastNotificationComponent],
  imports: [
    CommonModule,
    NotificationRoutingModule,
    ReactiveFormsModule,
    MaterialCommonModule,
    PaginationComponent,
    TableViewModule,
    TranslateModule,
    FilterComponent,
    SharedModule,
    AutoCompleteUserComponent,
  ],
  providers: [NotificationServiceProxy],
})
export class NotificationModule {}
