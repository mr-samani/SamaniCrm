import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import {
  ProductServiceProxy,
  CreateOrUpdateProductCategoryCommand,
} from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { ProductCategoryTranslationDto } from '@shared/service-proxies/model/product-category-translation-dto';

@Component({
  selector: 'create-or-edit-categorycreate-or-edit',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
  standalone: false,
})
export class CreateOrEditProductCategoryComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: ProductCategoryTranslationDto[];
  id: string;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: { id: string },
    private dialogRef: MatDialogRef<CreateOrEditProductCategoryComponent>,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      slug: ['', [Validators.maxLength(100)]],
      orderIndex: [0, [Validators.required]],
      image: [''],
      parentId: [],
      translations: this.fb.array([]),
      isActive: [true],
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
        new ProductCategoryTranslationDto({
          culture: item.culture!,
          title: '',
          description: '',
          productCategoryId: this.id,
        }),
      );
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.productService
      .getProductCategoryForEdit(id)
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
          description: [translation.description],
          productCategoryId: [translation.productCategoryId],
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
    const input = new CreateOrUpdateProductCategoryCommand();
    input.init(this.form.value);
    input.id = this.id;
    this.productService
      .createOrEditProductCategory(input)
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
