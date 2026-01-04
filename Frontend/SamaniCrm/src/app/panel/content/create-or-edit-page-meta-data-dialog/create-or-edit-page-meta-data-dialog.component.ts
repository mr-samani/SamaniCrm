
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormGroup, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { AppConst } from '@shared/app-const';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import {
  PageStatusEnum,
  PagesServiceProxy,
  CreateOrEditPageMetaDataCommand,
  PageMetaDataDto,
  PageTypeEnum,
} from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-create-or-edit-page-meta-data-dialog',
  templateUrl: './create-or-edit-page-meta-data-dialog.component.html',
  styleUrls: ['./create-or-edit-page-meta-data-dialog.component.scss'],
  imports: [TabGroupModule, ReactiveFormsModule, TranslateModule, SharedModule, MaterialCommonModule],
})
export class CreateOrEditPageMetaDataDialogComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: PageMetaDataDto[];
  id: string;
  type: PageTypeEnum;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: { id: string; type: PageTypeEnum },
    private dialogRef: MatDialogRef<CreateOrEditPageMetaDataDialogComponent>,
    private pageService: PagesServiceProxy,
  ) {
    super(injector);
    this.type = _data.type;
    this.form = this.fb.group({
      translations: this.fb.array([]),
      isActive: [true],
      isSystem: [{ value: false, disabled: true }],
      status: [PageStatusEnum.Draft, [Validators.required]],
    });

    this.id = _data.id;

    if (this.id) {
      this.isUpdate = true;
      this.getForEdit(this.id);
    } else {
      this.isUpdate = false;
      this.getForCreate();
    }
  }

  ngOnInit(): void {}

  public get PageStatusEnum(): typeof PageStatusEnum {
    return PageStatusEnum;
  }
  getForCreate() {
    this.translations = [];
    for (let item of AppConst.languageList ?? []) {
      this.translations.push(
        new PageMetaDataDto({
          culture: item.culture!,
          title: '',
          introduction: '',
          description: '',
          metaDescription: '',
          metaKeywords: '',
        }),
      );
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.pageService
      .getForEditMetaData(id)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.form.patchValue(response.data);
            this.translations = response.data.translations;
            this.setTranslations();
          } else {
            this.dialogRef.close();
          }
        },
        error: (err) => {
          this.dialogRef.close();
        },
      });
  }

  get translationsArray(): FormArray {
    return this.form.get('translations') as FormArray;
  }

  setTranslations() {
    if (!this.translations) {
      return;
    }
    this.translations.forEach((translation) => {
      this.translationsArray.push(
        this.fb.group({
          culture: [translation.culture],
          // data: this.fb.group({
          title: [translation.title, Validators.required],
          introduction: [],
          description: [],
          metaDescription: [],
          metaKeywords: [],
          //})
        }),
      );
    });
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormField'));
      return;
    }
    this.saving = true;
    const input = new CreateOrEditPageMetaDataCommand();
    input.init(this.form.value);
    input.id = this.id;
    input.type = this.type;
    this.pageService
      .createOrEditPageMetaData(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.dialogRef.close(true);
          }
        },
      });
  }
}
