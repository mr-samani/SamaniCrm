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

@NgModule({
  declarations: [ProductComponent, ProductCategoriesComponent],
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
  ],
  providers: [ProductServiceProxy],
})
export class ProductModule {}
