import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import {
  ProductServiceProxy,
  ProductTypeTranslationDto,
  CreateOrUpdateProductCommand,
  ProductTranslationDto,
  ProductAttributeValueDto,
  ProductFileDto,
  ProductImageDto,
  ProductPriceDto,
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
  attributeValues?: Array<ProductAttributeValueDto>;
  images?: Array<ProductImageDto>;
  files?: Array<ProductFileDto>;
  prices?: Array<ProductPriceDto>;
  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('Products'), url: '/dashboard/products/product-list' }];

    this.form = this.fb.group({
      category: ['', [Validators.required]],
      productType: ['', [Validators.required]],
      sku: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      slug: ['', [Validators.required]],
      isActive: [true, [Validators.required]],
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
            this.form.get('category')?.patchValue({
              id: response.data.categoryId,
              title: response.data.categoryTitle,
            });
            this.form.get('productType')?.patchValue({
              id: response.data.productTypeId,
              title: response.data.productTypeTitle,
            });
            this.translations = response.data.translations;
            this.attributeValues = response.data.attributeValues ?? [];
            this.files = response.data.files ?? [];
            this.images = response.data.images ?? [];
            this.prices = response.data.prices ?? [];

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
    const formValue = this.form.value;
    const input = new CreateOrUpdateProductCommand();
    input.init(formValue);
    input.id = this.id;
    input.categoryId = formValue.category.id;
    input.productTypeId = formValue.productType.id;
    input.files = this.files;
    input.attributeValues = this.attributeValues;
    input.images = this.images;
    input.prices = this.prices;

    this.productService
      .createOrEditProduct(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.router.navigate(['/dashboard/products/product-list']);
          }
        },
      });
  }
}
