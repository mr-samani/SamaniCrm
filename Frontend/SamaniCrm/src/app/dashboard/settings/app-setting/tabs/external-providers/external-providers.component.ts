import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import { AccountServiceProxy, ExternalProviderDto, SecuritySettingsServiceProxy } from '@shared/service-proxies';
import { finalize, Subscription } from 'rxjs';
import { CreateOrEditExternalProviderComponent } from '../../dialogs/external-provider/external-provider.component';

@Component({
  selector: 'app-external-providers',
  templateUrl: './external-providers.component.html',
  styleUrls: ['./external-providers.component.scss'],
  standalone: false,
})
export class ExternalProvidersComponent extends AppComponentBase implements OnInit, OnDestroy {
  loading = true;

  list: ExternalProviderDto[] = [];
  totalCount = 0;
  fields: FieldsType[] = [
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'name', title: this.l('Name') },
    { column: 'displayName', title: this.l('Title') },
    { column: 'externalProviderType', title: this.l('Type'), type: 'enum' },
    { column: 'authorizationEndpoint', title: this.l('AuthorizationEndpoint') },
    { column: 'tokenEndpoint', title: this.l('TokenEndpoint') },
    { column: 'isActive', title: this.l('IsActive'), type: 'yesNo' },
  ];

  listSubscription$?: Subscription;
  showFilter = false;

  constructor(
    injector: Injector,
    private securitySettingsService: SecuritySettingsServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);

    this.getList();
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

    this.listSubscription$ = this.securitySettingsService
      .getAllExternalProviders()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? [];
        this.totalCount = response.data?.length ?? 0;
      });
  }

  changeIsActive(item: ExternalProviderDto) {
    this.confirmMessage(`${item?.name}`, this.l('AreYouSureForChange')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.securitySettingsService
          .changeIsActiveExternalProvider(item.id, item.isActive)
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe({
            next: (response) => {
              this.notify.success(this.l('ChangedSuccessfully'));
            },
            error: (err) => {
              item.isActive = !item.isActive;
            },
          });
      } else {
        item.isActive = !item.isActive;
      }
    });
  }

  openCreateOrEditDialog(item?: ExternalProviderDto) {
    this.matDialog
      .open(CreateOrEditExternalProviderComponent, {
        data: {
          provider: item,
        },
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          // Reload list
          this.getList();
        }
      });
  }
}
