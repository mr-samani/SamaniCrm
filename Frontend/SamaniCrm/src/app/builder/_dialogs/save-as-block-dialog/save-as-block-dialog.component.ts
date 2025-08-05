import { CommonModule } from '@angular/common';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { BlockDefinition } from '@app/builder/blocks/block-registry';
import { getBlocksAsString } from '@app/builder/helpers/get-blocks-as-string';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { CreateCustomBlockCommand, PageBuilderServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-save-as-block-dialog',
  templateUrl: './save-as-block-dialog.component.html',
  styleUrls: ['./save-as-block-dialog.component.scss'],
  standalone: true,
  imports: [CommonModule, TranslateModule, MaterialCommonModule, ReactiveFormsModule],
  providers: [PageBuilderServiceProxy],
})
export class SaveAsBlockDialogComponent extends AppComponentBase implements OnInit {
  isUpdate = false;
  loading = false;
  saving = false;

  form: FormGroup;
  constructor(
    injector: Injector,
    private matDialogRef: MatDialogRef<SaveAsBlockDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { block: BlockDefinition },
    private pageBuilderService: PageBuilderServiceProxy,
  ) {
    super(injector);
    let block = this.data.block;
    this.form = this.fb.group({
      name: ['', [Validators.required]],
      description: [''],
      icon: [''],
      image: [''],
      categoryName: [''],
      data: [''],
    });
    this.form.get('data')?.patchValue(getBlocksAsString(block));
  }

  ngOnInit() {}

  save() {
    if (this.form.invalid) {
      this.notify.warning(this.l('CompleteFormFields'));
      this.form.markAllAsTouched();
      return;
    }
    this.saving = true;
    const formValue = this.form.value;
    const input = new CreateCustomBlockCommand();
    input.init(formValue);
    this.pageBuilderService
      .saveAsBlockDefinition(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        this.notify.success(this.l('SavedSuccessfully'));
        this.matDialogRef.close(true);
      });
  }
}
