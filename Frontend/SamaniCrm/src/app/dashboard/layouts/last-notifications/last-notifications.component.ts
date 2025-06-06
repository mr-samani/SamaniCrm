import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { NotificationInfoComponent } from '@app/dashboard/notifications/notification-info/notification-info.component';
import { NotificationDto, NotificationServiceProxy } from '@shared/service-proxies';
import { NotificationService } from '@shared/services/notification.service';
import { DateTime } from 'luxon';
import { finalize } from 'rxjs';

@Component({
  selector: 'last-notifications',
  templateUrl: './last-notifications.component.html',
  styleUrls: ['./last-notifications.component.scss'],
  standalone: false,
})
export class LastNotificationsComponent extends AppComponentBase implements OnInit {
  count = 0;

  notificationList: NotificationDto[] = [];
  loading = true;

  constructor(
    injector: Injector,
    notificationService: NotificationService,
    private notificationServiceProxy: NotificationServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);
    notificationService.startConnection().then((result) => {
      notificationService.onReceiveNotification((msg: NotificationDto) => this.recieveMessage(msg));
    });
  }

  ngOnInit() {
    this.getLastNotifications();
  }

  recieveMessage(msg: NotificationDto) {
    this.notificationList.unshift(msg);
    this.count = this.notificationList.length;
  }

  getLastNotifications() {
    this.loading = true;
    this.notificationServiceProxy
      .getLastUnReadNotifications()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.notificationList = result.data ?? [];
      });
  }

  markAllAsRead() {
    this.notificationServiceProxy.markAllAsRead().subscribe();
  }

  openNotify(item: NotificationDto) {
    this.matDialog
      .open(NotificationInfoComponent, {
        data: item,
      })
      .afterClosed()
      .subscribe((r) => {
        item.read = true;
      });
  }
}
