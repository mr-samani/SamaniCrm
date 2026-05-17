import { ChangeDetectorRef, Component, inject, OnInit, output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ChangeTenantDialogComponent } from './change-tenant-dialog/change-tenant-dialog.component';
import { ChangeTenantResult } from './change-tenant-dialog/ChangeTenantResult';
import { AppConst } from '@shared/app-const';

const TENANCY_NAME_KEY = 'tenancyName';

@Component({
  selector: 'select-tenant',
  templateUrl: './select-tenant.component.html',
  styleUrls: ['./select-tenant.component.scss'],
  standalone: false,
})
export class SelectTenantComponent implements OnInit {
  tenancyName = '';
  tenantId = '';
  matDialog = inject(MatDialog);

  onChangeTenant = output<string>();
  chdr = inject(ChangeDetectorRef);
  constructor() {
    const c = localStorage.getItem(TENANCY_NAME_KEY) || '';
    if (c) {
      this.tenancyName = c;
      AppConst.tenancyName = this.tenancyName;
    }
  }

  ngOnInit() {}

  changeTenant() {
    this.matDialog
      .open(ChangeTenantDialogComponent, {
        data: {},
      })
      .afterClosed()
      .subscribe((result: ChangeTenantResult) => {
        if (result) {
          this.tenancyName = result.tenancyName;
          this.tenantId = result.tenantId;
          AppConst.tenancyName = this.tenancyName;
          AppConst.tenantId = this.tenantId;

          localStorage.setItem(TENANCY_NAME_KEY, this.tenancyName);
          this.onChangeTenant.emit(this.tenancyName);
          this.chdr.detectChanges();
        }
      });
  }
}
