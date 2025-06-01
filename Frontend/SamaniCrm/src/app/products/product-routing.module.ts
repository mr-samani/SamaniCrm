import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductComponent } from './product.component';
import { ProductCategoriesComponent } from './product-categories/product-categories.component';
import { ProductTypesComponent } from './product-types/product-types.component';

const routes: Routes = [
  {
    path: '',
    component: ProductComponent,
    children: [
      { path: '', redirectTo: 'categories', pathMatch: 'full' },
      {
        path: 'categories',
        component: ProductCategoriesComponent,
      },
      {
        path: 'types',
        component: ProductTypesComponent,
      },
      {
        path: 'attributes/:productTypeId',
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProductRoutingModule {}
