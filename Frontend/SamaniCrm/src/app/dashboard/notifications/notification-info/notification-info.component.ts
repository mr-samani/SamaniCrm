import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { NotificationDto, NotificationServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-notification-info',
  templateUrl: './notification-info.component.html',
  styleUrls: ['./notification-info.component.scss'],
  standalone: false,
})
export class NotificationInfoComponent extends AppComponentBase implements OnInit {
  loading = true;
  id = '';
  notifyInfo?: NotificationDto;
  constructor(
    injector: Injector,
    private dialogRef: MatDialogRef<NotificationInfoComponent>,
    @Inject(MAT_DIALOG_DATA) _data: NotificationDto,
    private notificationService: NotificationServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getInfo();
  }

  getInfo() {
    this.loading = true;
    this.notificationService
      .getNotificationInfo(this.id)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.notifyInfo = result.data;
      });
  }
}
