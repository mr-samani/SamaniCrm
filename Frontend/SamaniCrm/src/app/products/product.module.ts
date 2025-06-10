import { ProductImagesComponent } from './products/create-or-edit/product-images/product-images.component';
import { ProductPricesComponent } from './products/create-or-edit/product-prices/product-prices.component';
import { CurrenciesComponent } from './currencies/currencies.component';
import { AddOrUpdateProductAttributesComponent } from './products/create-or-edit/add-or-update-product-attributes/add-or-update-product-attributes.component';
import { ProductsComponent } from './products/products.component';
import { ProductAttributesComponent } from './product-attributes/product-attributes.component';
import { ProductTypesComponent } from './product-types/product-types.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductRoutingModule } from './product-routing.module';
import { ProductComponent } from './product.component';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { TableViewModule } from '@shared/components/table-view/table-view.module';
import { FilterComponent } from '@shared/components/filter/filter.component';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { ProductCategoriesComponent } from './product-categories/product-categories.component';
import { ProductServiceProxy } from '@shared/service-proxies/api/product.service';
import { CreateOrEditProductCategoryComponent } from './product-categories/create-or-edit/create-or-edit.component';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { CreateOrEditProductTypeComponent } from './product-types/create-or-edit/create-or-edit.component';
import { CreateOrEditProductAttributeComponent } from './product-attributes/create-or-edit/create-or-edit.component';
import { CreateOrEditProductComponent } from './products/create-or-edit/create-or-edit.component';
import { AutoCompleteProductCategoryComponent } from './auto-completes/auto-complete-category/auto-complete-category.component';
import { AutoCompleteProductTypeComponent } from './auto-completes/auto-complete-product-type/auto-complete-product-type.component';
import { MatChipsModule } from '@angular/material/chips';
import { HtmlEditorModule } from '@shared/components/html-editor/html-editor.module';
import { CreateOrEditCurrencyComponent } from './currencies/create-or-edit/create-or-edit.component';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { ProductFilesComponent } from './products/create-or-edit/product-files/product-files.component';

@NgModule({
  declarations: [
    ProductComponent,
    ProductCategoriesComponent,
    CreateOrEditProductCategoryComponent,
    ProductTypesComponent,
    CreateOrEditProductTypeComponent,
    ProductAttributesComponent,
    CreateOrEditProductAttributeComponent,
    ProductsComponent,
    CreateOrEditProductComponent,
    AddOrUpdateProductAttributesComponent,
    CurrenciesComponent,
    CreateOrEditCurrencyComponent,
    ProductPricesComponent,
    ProductImagesComponent,
    ProductFilesComponent,
  ],
  imports: [
    CommonModule,
    ProductRoutingModule,
    TranslateModule,
    ReactiveFormsModule,
    SharedModule,
    PaginationComponent,
    TableViewModule,
    FilterComponent,
    MaterialCommonModule,
    TabGroupModule,
    AutoCompleteProductCategoryComponent,
    AutoCompleteProductTypeComponent,
    MatChipsModule,
    HtmlEditorModule,
    FormsModule,
    FileManagerModule,
  ],
  providers: [ProductServiceProxy],
})
export class ProductModule {}
