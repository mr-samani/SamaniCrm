
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import {
  CreateOrEditLanguageCommand,
  CreateOrUpdateDashboardCommand,
  DasboardServiceProxy,
} from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-create-dashboard',
  templateUrl: './create-dashboard.component.html',
  styleUrls: ['./create-dashboard.component.scss'],
  standalone: true,
  imports: [SharedModule, MaterialCommonModule, ReactiveFormsModule],
  providers:[
    DasboardServiceProxy
  ]
})
export class CreateDashboardComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  isUpdate: boolean;
  saving = false;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: { id?: string },
    private dialogRef: MatDialogRef<CreateDashboardComponent>,
    private dashboardService: DasboardServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      order: ['', [Validators.required]],
      isPublic: [false, []],
    });
    if (_data) {
      this.isUpdate = true;
      this.form.patchValue(_data);
    } else {
      this.isUpdate = false;
    }
  }

  ngOnInit() {}

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }

    this.saving = true;
    const input = new CreateOrUpdateDashboardCommand();
    input.init(this.form.value);
    this.dashboardService
      .createOrUpdateDashboard(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success('SavedSuccessfully');
          this.dialogRef.close(true);
        }
      });
  }
}
