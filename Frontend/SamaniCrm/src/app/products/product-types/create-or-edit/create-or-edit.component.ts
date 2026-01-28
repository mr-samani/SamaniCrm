import { Component, Inject,  OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import {
  ProductServiceProxy,
  ProductTypeTranslationDto,
  CreateOrUpdateProductTypeCommand,
} from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'create-or-edit-product-type',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
  standalone: false,
})
export class CreateOrEditProductTypeComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: ProductTypeTranslationDto[];
  id: string;
  constructor(
    @Inject(MAT_DIALOG_DATA) _data: { id: string },
    private dialogRef: MatDialogRef<CreateOrEditProductTypeComponent>,
    private productService: ProductServiceProxy,
  ) {
    super();
    this.form = this.fb.group({
      translations: this.fb.array([]),
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

  getForCreate() {
    this.translations = [];
    for (let item of AppConst.languageList ?? []) {
      this.translations.push(
        new ProductTypeTranslationDto({
          culture: item.culture!,
          name: '',
          description: '',
          productTypeId: this.id,
        }),
      );
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.productService
      .getProductTypeForEdit(id)
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
          name: [translation.name, Validators.required],
          description: [translation.description],
          productTypeId: [translation.productTypeId],
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
    const input = new CreateOrUpdateProductTypeCommand();
    input.init(this.form.value);
    input.id = this.id;
    this.productService
      .createOrEditProductType(input)
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
