import { Component, Injector, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import { CurrencyDto, DeleteCurrencyCommand, ProductServiceProxy } from '@shared/service-proxies';
import { finalize, Subscription } from 'rxjs';
import { CreateOrEditCurrencyComponent } from './create-or-edit/create-or-edit.component';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-currencies',
  templateUrl: './currencies.component.html',
  styleUrls: ['./currencies.component.scss'],
  standalone: false,
})
export class CurrenciesComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: CurrencyDto[] = [];
  totalCount = 0;
  fields: FieldsType[] = [
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'currencyCode', title: this.l('Code') },
    { column: 'name', title: this.l('Name') },
    { column: 'symbol', title: this.l('Symbol') },
    { column: 'exchangeRateToBase', title: this.l('ExchangeRateToBase'), type: 'number' },
    { column: 'isDefault', title: this.l('IsDefault'), type: 'yesNo' },
    { column: 'isActive', title: this.l('IsActive'), type: 'yesNo' },
    // { column: 'creationTime', title: this.l('CreationTime'), type: 'dateTime' },
  ];
  form: FormGroup;
  page = 1;
  perPage = AppConst.defaultTablePerPage;
  listSubscription$?: Subscription;
  showFilter = false;

  constructor(
    injector: Injector,
    private productService: ProductServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('Currencies'), url: '/panel/products/currencies' }];
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
    this.productService
      .getCurrencies()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? [];
        this.totalCount = this.list.length;
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
    this.router.navigate(['/panel/products/currencies'], {
      queryParams: {
        page: this.page,
      },
    });
  }

  openCreateOrEditDialog(item?: CurrencyDto) {
    this.matDialog
      .open(CreateOrEditCurrencyComponent, {
        data: item,
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.reload();
        }
      });
  }

  remove(item: CurrencyDto) {
    this.confirmMessage(`${this.l('Delete')}:${item?.name}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.productService
          .deleteCurrency(new DeleteCurrencyCommand({ id: item.id }))
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
