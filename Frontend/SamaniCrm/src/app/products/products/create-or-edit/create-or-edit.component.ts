import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import {
  ProductServiceProxy,
  ProductTypeTranslationDto,
  CreateOrUpdateProductCommand,
  ProductTranslationDto,
} from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'create-or-edit-product',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
  standalone: false,
})
export class CreateOrEditProductComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: ProductTranslationDto[];
  id: string;

  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('Products'), url: '/dashboard/products/product-list' }];

    this.form = this.fb.group({
      translations: this.fb.array([]),
    });
    this.id = this.route.snapshot.params['id'];

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
        new ProductTranslationDto({
          culture: item.culture!,
          title: '',
          description: '',
          productId: this.id,
        }),
      );
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.productService
      .getProductForEdit(id)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.form.patchValue(response.data);
            this.translations = response.data.translations;
            this.setTranslations();
          } else {
            window.history.back();
          }
        },
        error: (err) => {
          window.history.back();
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
          productId: [translation.productId],
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
    const input = new CreateOrUpdateProductCommand();
    input.init(this.form.value);
    input.id = this.id;
    this.productService
      .createOrEditProduct(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.router.navigate(['/app/dashboard/products/product-list']);
          }
        },
      });
  }
}
