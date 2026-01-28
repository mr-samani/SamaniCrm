import { Component,  OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import {
  DeleteProductAttributeCommand,
  GetProductAttributesQuery,
  ProductAttributeDto,
  ProductServiceProxy,
} from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { Subscription } from 'rxjs/internal/Subscription';
import { CreateOrEditProductAttributeComponent } from './create-or-edit/create-or-edit.component';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-product-attributes',
  templateUrl: './product-attributes.component.html',
  styleUrls: ['./product-attributes.component.scss'],
  standalone: false,
})
export class ProductAttributesComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: ProductAttributeDto[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'name', title: this.l('Name') },
    { column: 'dataType', title: this.l('DataType') },
    { column: 'isRequired', title: this.l('IsRequired'), type: 'yesNo' },
    { column: 'isVariant', title: this.l('IsVariant'), type: 'yesNo' },
    { column: 'sortOrder', title: this.l('SortOrder') },
    { column: 'creationTime', title: this.l('CreationTime'), type: 'dateTime' },
  ];

  form: FormGroup;
  page = 1;
  perPage = AppConst.defaultTablePerPage;
  listSubscription$?: Subscription;
  showFilter = false;

  productTypeId = '';
  productTypeName = '';

  constructor(
    private productService: ProductServiceProxy,
    private matDialog: MatDialog,
  ) {
    super();
    this.productTypeName = this.route.snapshot.queryParams['name'];
    this.productTypeId = this.route.snapshot.params['productTypeId'];
    this.breadcrumb.list = [
      { name: this.l('ProductTypes'), url: '/panel/products/types' },
      { name: this.productTypeName },
    ];
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
    const input = new GetProductAttributesQuery();
    input.filter = this.form.get('filter')?.value;
    input.productTypeId = this.productTypeId;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
    this.listSubscription$ = this.productService
      .getProductAttributes(input)
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
    if (ev) this.perPage = ev.perPage;
    this.getList();
    this.router.navigate(['/panel/products/attributes/' + this.productTypeId], {
      queryParams: {
        page: this.page,
        name: this.productTypeName,
      },
    });
  }

  openCreateOrEditDialog(item?: ProductAttributeDto) {
    this.matDialog
      .open(CreateOrEditProductAttributeComponent, {
        data: {
          id: item?.id,
          productTypeId: this.productTypeId,
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

  remove(item: ProductAttributeDto) {
    this.confirmMessage(`${this.l('Delete')}:${item?.name}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.productService
          .deleteProductAttribute(new DeleteProductAttributeCommand({ id: item.id }))
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
