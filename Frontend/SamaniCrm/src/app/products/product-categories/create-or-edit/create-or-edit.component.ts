import { CommonModule } from '@angular/common';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormArray, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { Apis } from '@shared/apis';
import { TreeCategoryComponent } from '../tree-category/tree-category.component';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { GetCategoryForEditDto, ProductCategoryTranslation } from '@app/products/models/get-product-category-for-edit';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { finalize } from 'rxjs';
import { LanguageDto } from '@shared/models/language-dto';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'create-or-edit-categorycreate-or-edit',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    TreeCategoryComponent,
    MaterialCommonModule,
    TranslateModule,
    ReactiveFormsModule,
    TabGroupModule,
  ],
})
export class CreateOrEditProductCategoryComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: ProductCategoryTranslation[];
  id: string;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: any,
    private dialogRef: MatDialogRef<CreateOrEditProductCategoryComponent>,
  ) {
    super(injector);
    this.form = this.fb.group({
      translations: this.fb.array([]),
    });
    this.id = _data.id;

    if (_data.id) {
      this.isUpdate = true;
      this.getForEdit(_data.id);
    } else {
      this.isUpdate = false;
      this.getForCreate();
    }
  }

  ngOnInit(): void {}

  getForCreate() {
    this.translations = [];
    for (let item of AppConst.languageList ?? []) {
      this.translations.push({
        id: undefined,
        lang: item.code,
        name: '',
        description: '',
      });
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.dataService
      .get<{ id: string }, GetCategoryForEditDto>(Apis.getProductCategoryForEdit, { id })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (response) => {
          this.translations = response.result.translations;
          this.setTranslations();
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
          lang: [translation.lang, Validators.required],
          // data: this.fb.group({
          id: [translation.id, Validators.required],
          name: [translation.name, Validators.required],
          description: [translation.description],
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
    const input = this.form.value;
    input.categoryId = this.id;
    this.dataService
      .post(Apis.createOrEditProductCategory, input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.dialogRef.close(true);
          } else {
            let msg = response.message ?? this.l('Message.ErrorOccurred');
            this.notify.error(msg);
          }
        },
      });
  }
}
