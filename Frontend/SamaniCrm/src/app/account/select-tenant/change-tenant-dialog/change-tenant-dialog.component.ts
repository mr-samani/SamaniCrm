import { Component, inject, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { ChangeTenantResult } from './ChangeTenantResult';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-change-tenant-dialog',
  templateUrl: './change-tenant-dialog.component.html',
  styleUrls: ['./change-tenant-dialog.component.scss'],
  standalone: false,
})
export class ChangeTenantDialogComponent extends AppComponentBase implements OnInit {
  tenancyName = '';
  dialogRef = inject(MatDialogRef<ChangeTenantDialogComponent>);
  constructor() {
    super();
  }

  ngOnInit() {}

  ok() {
    this.tenancyName = this.tenancyName.trim();
    this.dialogRef.close(<ChangeTenantResult>{
      tenancyName: this.tenancyName,
    });
  }
}
