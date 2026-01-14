import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { DasboardServiceProxy } from '@shared/service-proxies/api/dasboard.service';
import { CreateOrUpdateDashboardItemCommand } from '@shared/service-proxies/model/create-or-update-dashboard-item-command';

import { SharedModule } from '@shared/shared.module';
import { finalize } from 'rxjs';
import { IWidgetDefinition, Widget, WidgetHelper } from '../widgets/widgets';

@Component({
  selector: 'app-add-widget',
  templateUrl: './add-widget.component.html',
  styleUrls: ['./add-widget.component.scss'],
  standalone: true,
  imports: [SharedModule, MaterialCommonModule, ReactiveFormsModule],
  providers: [DasboardServiceProxy],
})
export class AddDashboardWidgetComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  saving = false;

  widgets = WidgetHelper.WidgetDefinition;

  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) private _data: { dashboardId: string },
    private dialogRef: MatDialogRef<AddDashboardWidgetComponent>,
    private dashboardService: DasboardServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      dashboardId: [_data.dashboardId, [Validators.required, Validators.maxLength(100)]],
      componentName: ['', [Validators.required]],
      data: [''],
      position: [''],
    });
  }

  ngOnInit() {}

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }

    this.saving = true;
    const input = new CreateOrUpdateDashboardItemCommand();
    input.init(this.form.value);
    this.dashboardService
      .createOrUpdateDashboardItem(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success('SavedSuccessfully');
          this.dialogRef.close(true);
        }
      });
  }
}
