import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { NotificationService } from '@shared/services/notification.service';

@Component({
  selector: 'last-notifications',
  templateUrl: './last-notifications.component.html',
  styleUrls: ['./last-notifications.component.scss'],
  standalone: false,
})
export class LastNotificationsComponent extends AppComponentBase implements OnInit {
  count = 0;

  notificationList: string[] = [];

  constructor(
    injector: Injector,
    private notificationService: NotificationService,
  ) {
    super(injector);
    notificationService.startConnection().then((result) => {
      notificationService.onReceiveNotification((msg: string) => this.recieveMessage(msg));
    });
  }

  ngOnInit() {}

  recieveMessage(msg: string) {
    this.notificationList.unshift(msg);
    this.count = this.notificationList.length;
  }
}
