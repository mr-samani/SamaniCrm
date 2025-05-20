import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { TreeCategoryComponent, TreeNode } from './tree-category/tree-category.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { ProductCategoryDto, ProductServiceProxy } from '@shared/service-proxies';

@Component({
  selector: 'app-product-categories',
  standalone: true,
  imports: [
    CommonModule,
    TreeCategoryComponent,
    DragDropModule,
    MatProgressBarModule,
    TranslateModule,
    MatButtonModule,
  ],
  templateUrl: './product-categories.component.html',
  styleUrl: './product-categories.component.scss',
  providers: [ProductServiceProxy],
})
export class ProductCategoriesComponent extends AppComponentBase implements OnInit {
  list: TreeNode[] = [];
  loading = true;
  isSaving = false;
  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.loading = true;
    this.productService
      .getCategoriesForAdmin()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? ([] as any);
      });
  }

  save() {
    // this.isSaving = true;
    // this.dataService
    //   .post<{ categories: ProductCategory[] }, null>(Apis.reorderProductCategories, { categories: this.list })
    //   .pipe(finalize(() => (this.isSaving = false)))
    //   .subscribe((response) => {
    //     this.notify.success(this.l('SaveSuccessFully'));
    //     this.getList();
    //   });
  }
}
