import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AccountServiceProxy, Verify2FARequest } from '@shared/service-proxies';
import { finalize, map } from 'rxjs';

@Component({
  selector: 'app-two-fa-app-config',
  templateUrl: './two-fa-app-config.component.html',
  styleUrls: ['./two-fa-app-config.component.scss'],
  standalone: false,
})
export class TwoFaAppConfigComponent extends AppComponentBase implements OnInit {
  loading = false;
  saving = false;
  code = '';
  img = '';
  secret = '';

  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: any,
    private dialogRef: MatDialogRef<TwoFaAppConfigComponent>,
    private accountService: AccountServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getQrCode();
  }

  getQrCode() {
    this.loading = true;
    this.accountService
      .generate2FaRequestGenerate()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        if (result.data) {
          this.img = result.data.qrCode ?? '';
          this.secret = result.data.secret ?? '';
        }
      });
  }

  save() {
    if (!this.code) {
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }
    this.saving = true;
    this.accountService
      .verify2FaApp(
        new Verify2FARequest({
          code: this.code,
          secret: this.secret,
        }),
      )
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        if (result.data == true) {
          this.notify.success(this.l('SaveSuccessFully'));
          this.dialogRef.close(true);
        } else {
          this.notify.error(this.l('Invalid2FaCode'));
        }
      });
  }
}
