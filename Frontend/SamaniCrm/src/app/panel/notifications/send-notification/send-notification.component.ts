import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import {
  NotificationDto,
  NotificationPeriorityEnum,
  NotificationServiceProxy,
  SendNotificationCommand,
} from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'send-notification',
  templateUrl: './send-notification.component.html',
  styleUrls: ['./send-notification.component.scss'],
  standalone: false,
})
export class SendNotificationDialogComponent extends AppComponentBase implements OnInit {
  saving = false;
  notifyInfo?: NotificationDto;
  form: FormGroup;
  constructor(
    injector: Injector,
    private dialogRef: MatDialogRef<SendNotificationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) _data: NotificationDto,
    private notificationService: NotificationServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      user: ['', [Validators.required]],
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
    const input = new SendNotificationCommand();
    input.userId = formValue.user.id;
    input.title = formValue.title;
    input.content = formValue.content;
    input.periority = formValue.periority;

    this.notificationService
      .sendMessageToUser(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        if (result.success) {
          this.notify.success(this.l('SentSuccessfully'));
          this.dialogRef.close(true);
        }
      });
  }
}
