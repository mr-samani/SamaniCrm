import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import {
  ProductServiceProxy,
  ProductAttributeTranslationDto,
  CreateOrUpdateProductAttributeCommand,
  ProductAttributeDataTypeEnum,
} from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'create-or-edit-product-attribute',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
  standalone: false,
})
export class CreateOrEditProductAttributeComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: ProductAttributeTranslationDto[];
  id: string;
  productTypeId: string;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: { id: string; productTypeId: string },
    private dialogRef: MatDialogRef<CreateOrEditProductAttributeComponent>,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      dataType: [ProductAttributeDataTypeEnum.String, [Validators.required]],
      isRequired: [true],
      isVariant: [false],
      sortOrder: ['', [Validators.required]],
      translations: this.fb.array([]),
    });
    this.id = _data.id;
    this.productTypeId = _data.productTypeId;

    if (this.id) {
      this.isUpdate = true;
      this.getForEdit(this.id);
    } else {
      this.isUpdate = false;
      this.getForCreate();
    }
  }

  ngOnInit(): void {}

  public get ProductAttributeDataTypeEnum(): typeof ProductAttributeDataTypeEnum {
    return ProductAttributeDataTypeEnum;
  }

  getForCreate() {
    this.translations = [];
    for (let item of AppConst.languageList ?? []) {
      this.translations.push(
        new ProductAttributeTranslationDto({
          culture: item.culture!,
          name: '',
          productAttributeId: this.id,
        }),
      );
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.productService
      .getProductAttributeForEdit(id)
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
          productAttributeId: [translation.productAttributeId],
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
    const input = new CreateOrUpdateProductAttributeCommand();
    input.init(this.form.value);
    input.id = this.id;
    input.productTypeId = this.productTypeId;
    this.productService
      .createOrEditProductAttribute(input)
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
