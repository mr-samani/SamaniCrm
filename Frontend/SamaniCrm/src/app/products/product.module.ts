import { ProductsComponent } from './products/products.component';
import { ProductAttributesComponent } from './product-attributes/product-attributes.component';
import { ProductTypesComponent } from './product-types/product-types.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductRoutingModule } from './product-routing.module';
import { ProductComponent } from './product.component';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { TableViewComponent } from '@shared/components/table-view/table-view.compoenent';
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
  ],
  imports: [
    CommonModule,
    ProductRoutingModule,
    TranslateModule,
    ReactiveFormsModule,
    SharedModule,
    PaginationComponent,
    TableViewComponent,
    FilterComponent,
    MaterialCommonModule,
    TabGroupModule,
    AutoCompleteProductCategoryComponent,
    AutoCompleteProductTypeComponent,
  ],
  providers: [ProductServiceProxy],
})
export class ProductModule {}
