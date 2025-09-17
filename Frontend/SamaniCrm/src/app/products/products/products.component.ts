import { Component, Injector, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import {
  ProductTypeDto,
  ProductServiceProxy,
  ProductDto,
  GetProductsQuery,
  DeleteProductCommand,
  ProductListDto,
} from '@shared/service-proxies';
import { Subscription, finalize } from 'rxjs';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss'],
  standalone: false,
})
export class ProductsComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: ProductListDto[] = [];
  totalCount = 0;
  publicSiteUrl = AppConst.publicSiteUrl;
  fields: FieldsType[] = [
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'title', title: this.l('Title') },
    { column: 'categoryTitle', title: this.l('Category') },
    { column: 'productTypeTitle', title: this.l('ProductType') },
    { column: 'description', title: this.l('Description') },
    { column: 'sku', title: this.l('Sku') },
    { column: 'slug', title: this.l('Slug') },
    { column: 'isActive', title: this.l('IsActive'), type: 'yesNo' },
    { column: 'creationTime', title: this.l('CreationTime'), type: 'dateTime' },
  ];

  form: FormGroup;
  page = 1;
  perPage = 10;
  listSubscription$?: Subscription;
  showFilter = false;

  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('Products'), url: '/dashboard/products/product-list' }];
    this.form = this.fb.group({
      filter: [''],
      productCategory: [],
      productType: [],
    });

    this.route.queryParams.subscribe((p) => {
      this.page = p['page'] ?? 1;
      this.perPage = p['perPage'] ?? 10;
      if (p['categoryId']) {
        this.form.get('productCategory')?.patchValue({
          title: p['categoryTitle'],
          id: p['categoryId'],
        });
      }
      if (p['typeId']) {
        this.form.get('productType')?.patchValue({
          title: p['typeTitle'],
          id: p['typeId'],
        });
      }
      this.getList();
    });
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
  }

  getList(ev?: SortEvent) {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
    this.loading = true;
    const formValue = this.form.value;
    const input = new GetProductsQuery();
    input.filter = formValue.filter;
    input.categoryId = formValue.productCategory ? formValue.productCategory.id : undefined;
    input.productTypeId = formValue.productType ? formValue.productType.id : undefined;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
    this.listSubscription$ = this.productService
      .getProducts(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data?.items ?? [];
        this.totalCount = response.data?.totalCount ?? 0;
      });
  }

  reload(setFirstPage = true) {
    if (setFirstPage) {
      this.page = 1;
    }
    this.onPageChange();
  }
  resetFilter() {
    this.showFilter = false;
    this.form.patchValue({ filter: '' });
    this.reload();
  }

  onPageChange(ev?: PageEvent) {
    this.getList();
    this.router.navigate(['/dashboard/products/product-list'], {
      queryParams: {
        page: this.page,
      },
    });
  }

  openCreateOrEditDialog(item?: ProductDto) {
    if (item) {
      this.router.navigate(['/dashboard/products/update-product/' + item.id]);
    } else {
      this.router.navigate(['/dashboard/products/new-product']);
    }
  }

  remove(item: ProductTypeDto) {
    this.confirmMessage(`${this.l('Delete')}:${item?.name}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.productService
          .deleteProduct(new DeleteProductCommand({ id: item.id }))
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            if (response.success) {
              this.notify.success(this.l('DeletedSuccessfully'));
              this.reload();
            }
          });
      }
    });
  }
}
