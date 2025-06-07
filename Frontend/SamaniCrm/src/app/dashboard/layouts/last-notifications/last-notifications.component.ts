import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { NotificationDto, NotificationServiceProxy } from '@shared/service-proxies';
import { NotificationService } from '@shared/services/notification.service';
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
  showHaveNewMessage = false;
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
    this.showNewMessage();
  }

  getLastNotifications() {
    this.loading = true;
    this.notificationServiceProxy
      .getLastUnReadNotifications()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.notificationList = result.data ?? [];
        this.count = this.notificationList.length;
      });
  }

  markAllAsRead() {
    this.notificationServiceProxy.markAllAsRead().subscribe();
  }

  async openNotify(item: NotificationDto) {
    const { NotificationInfoComponent } = await import(
      '@app/dashboard/notifications/notification-info/notification-info.component'
    );
    this.matDialog
      .open(NotificationInfoComponent, {
        data: item,
      })
      .afterClosed()
      .subscribe((r) => {
        if (item.read == false) {
          item.read = true;
          if (this.count > 0) this.count--;
        }
      });
  }

  showNewMessage() {
    this.showHaveNewMessage = true;
    setTimeout(() => {
      this.showHaveNewMessage = false;
    }, 5000);
  }
}
