import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import {
  BroadCastNotificationsCommand,
  NotificationDto,
  NotificationPeriorityEnum,
  NotificationServiceProxy,
} from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'broadcast-notification',
  templateUrl: './broadcast-notification.component.html',
  styleUrls: ['./broadcast-notification.component.scss'],
  standalone: false,
})
export class BroadcastNotificationComponent extends AppComponentBase implements OnInit {
  saving = false;
  notifyInfo?: NotificationDto;
  form: FormGroup;
  constructor(
    injector: Injector,
    private dialogRef: MatDialogRef<BroadcastNotificationComponent>,
    @Inject(MAT_DIALOG_DATA) _data: NotificationDto,
    private notificationService: NotificationServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      title: ['', [Validators.required]],
      content: ['', [Validators.required]],
      periority: [NotificationPeriorityEnum.Normal, [Validators.required]],
    });
  }

  ngOnInit() {}

  public get NotificationPeriorityEnum(): typeof NotificationPeriorityEnum {
    return NotificationPeriorityEnum;
  }
  save() {
    this.saving = true;
    const formValue = this.form.value;
    const input = new BroadCastNotificationsCommand();
    input.title = formValue.title;
    input.content = formValue.content;
    input.periority = formValue.periority;
    this.notificationService
      .broadCastMessageToAllUsers(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        if (result.success) {
          this.notify.success(this.l('SentSuccessfully'));
          this.dialogRef.close(true);
        }
      });
  }
}
