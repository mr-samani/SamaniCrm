import { Component, Injector, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import {
  ProductTypeDto,
  ProductServiceProxy,
  GetProductTypesQuery,
  DeleteProductTypeCommand,
} from '@shared/service-proxies';
import { Subscription, finalize } from 'rxjs';
import { CreateOrEditProductTypeComponent } from './create-or-edit/create-or-edit.component';

@Component({
  selector: 'app-product-types',
  templateUrl: './product-types.component.html',
  styleUrls: ['./product-types.component.scss'],
  standalone: false,
})
export class ProductTypesComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: ProductTypeDto[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    // { column: 'id', title: this.l('id'), width: 100 },
    { column: 'name', title: this.l('Name') },
    { column: 'description', title: this.l('Description') },
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
    this.breadcrumb.list = [{ name: this.l('ProductTypes'), url: '/dashboard/products/types' }];
    this.form = this.fb.group({
      filter: [''],
    });

    this.route.queryParams.subscribe((p) => {
      this.page = p['page'] ?? 1;
      this.perPage = p['perPage'] ?? 10;
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
    const input = new GetProductTypesQuery();
    input.filter = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
    this.listSubscription$ = this.productService
      .getProductTypes(input)
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
    this.router.navigate(['/dashboard/products/types'], {
      queryParams: {
        page: this.page,
      },
    });
  }

  openCreateOrEditDialog(item?: ProductTypeDto) {
    this.matDialog
      .open(CreateOrEditProductTypeComponent, {
        data: {
          id: item?.id,
        },
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.reload();
        }
      });
  }

  remove(item: ProductTypeDto) {
    this.confirmMessage(`${this.l('Delete')}:${item?.name}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.productService
          .deleteProductType(new DeleteProductTypeCommand({ id: item.id }))
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
