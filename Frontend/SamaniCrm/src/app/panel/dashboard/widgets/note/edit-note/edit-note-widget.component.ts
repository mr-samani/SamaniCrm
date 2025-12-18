import { CommonModule } from '@angular/common';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { DasboardServiceProxy } from '@shared/service-proxies/api/dasboard.service';
import { CreateOrUpdateDashboardItemCommand } from '@shared/service-proxies/model/create-or-update-dashboard-item-command';

import { SharedModule } from '@shared/shared.module';
import { finalize } from 'rxjs';
import { Widget } from '../../widgets';
import { WidgetNoteData } from '../WidgetNoteData';

@Component({
  selector: 'app-edit-note-widget',
  templateUrl: './edit-note-widget.component.html',
  styleUrls: ['./edit-note-widget.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule, MaterialCommonModule, ReactiveFormsModule],
  providers: [DasboardServiceProxy],
})
export class EditNoteWidgetComponent extends AppComponentBase implements OnInit {
  saving = false;

  widget: Widget<WidgetNoteData>;
  form: FormGroup;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) private _data: { item: Widget },
    private dialogRef: MatDialogRef<EditNoteWidgetComponent>,
    private dashboardService: DasboardServiceProxy,
  ) {
    super(injector);
    this.widget = _data.item;
    this.form = this.fb.group({
      title: [this.widget.data?.title, [Validators.required]],
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
    this.widget.data = this.form.value;
    input.init(this.widget);

    if (this.widget.position && typeof this.widget.position == 'object') {
      input.position = JSON.stringify(this.widget.position);
    }
    if (this.widget.data && typeof this.widget.data == 'object') {
      input.data = JSON.stringify(this.widget.data);
    }
    this.dashboardService
      .createOrUpdateDashboardItem(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success('SavedSuccessfully');

          this.dialogRef.close(this.widget);
        }
      });
  }
}
