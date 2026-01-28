import { Component, Inject,  OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { NotificationDto, NotificationServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-notification-info',
  templateUrl: './notification-info.component.html',
  styleUrls: ['./notification-info.component.scss'],
  standalone: true,
  providers: [NotificationServiceProxy],
  imports: [MaterialCommonModule, TranslateModule],
})
export class NotificationInfoComponent extends AppComponentBase implements OnInit {
  loading = true;
  id = '';
  notifyInfo?: NotificationDto;
  constructor(
    private dialogRef: MatDialogRef<NotificationInfoComponent>,
    @Inject(MAT_DIALOG_DATA) _data: NotificationDto,
    private notificationService: NotificationServiceProxy,
  ) {
    super();
    this.id = _data.id!;
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
