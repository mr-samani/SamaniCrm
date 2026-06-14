import { Component, OnInit } from '@angular/core';
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
    notificationService: NotificationService,
    private notificationServiceProxy: NotificationServiceProxy,
    private matDialog: MatDialog,
  ) {
    super();
    notificationService.startConnection();
    notificationService.onRecieveMessage$.subscribe((msg: NotificationDto) => this.recieveMessage(msg));
  }

  ngOnInit() {
    this.getLastNotifications();
  }

  recieveMessage(msg: NotificationDto) {
    this.notificationList.unshift(msg);
    this.count++;
    this.showNewMessage();
    this.chdr.detectChanges();
  }

  getLastNotifications() {
    this.loading = true;
    this.notificationServiceProxy
      .getLastUnReadNotifications()
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((result) => {
        this.notificationList = result.data?.items ?? [];
        this.count = result.data?.unreadCount ?? 0;
      });
  }

  markAllAsRead() {
    this.notificationServiceProxy.markAllAsRead().subscribe((result) => {
      if (result.success == true) {
        this.notificationList.map((m) => (m.read = true));
        this.count = 0;
      }
    });
  }

  async openNotify(item: NotificationDto) {
    const { NotificationInfoComponent } =
      await import('@app/notification-management/notification-info/notification-info.component');
    this.matDialog
      .open(NotificationInfoComponent, {
        data: item,
      })
      .afterClosed()
      .subscribe((r) => {
        if (item.read == false) {
          item.read = true;
          if (this.count > 0) this.count--;
          this.chdr.detectChanges();
        }
      });
  }

  showNewMessage() {
    this.showHaveNewMessage = true;
    this.chdr.detectChanges();
    setTimeout(() => {
      this.showHaveNewMessage = false;
      this.chdr.detectChanges();
    }, 5000);
  }
}
