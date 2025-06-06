import { Component, Injector, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType, SortEvent } from '@shared/components/table-view/fields-type.model';
import {
  DeleteNotificationCommand,
  GetAllNotificationQuery,
  NotificationDto,
  NotificationPeriorityEnum,
  NotificationServiceProxy,
  NotificationTypeEnum,
} from '@shared/service-proxies';
import { Subscription } from 'rxjs/internal/Subscription';
import { finalize } from 'rxjs/operators';
import { NotificationInfoComponent } from '../notification-info/notification-info.component';

@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.scss'],
  standalone: false,
})
export class NotificationListComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: NotificationDto[] = [];
  totalCount = 0;
  fields: FieldsType[] = [
    // { column: 'id', title: this.l('id'), width: 100 },
    { column: 'title', title: this.l('Title') },
    { column: 'type', title: this.l('Type'), type: 'localize', localizeKey: 'NotificationTypeEnum_' },
    { column: 'periority', title: this.l('Periority'), type: 'localize', localizeKey: 'NotificationPeriorityEnum_' },
    { column: 'read', title: this.l('Read'), type: 'template' },
    { column: 'creationTime', title: this.l('CreationTime'), type: 'dateTime' },
  ];
  form: FormGroup;
  page = 1;
  perPage = 10;
  listSubscription$?: Subscription;
  showFilter = false;

  constructor(
    injector: Injector,
    private notificationService: NotificationServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('Notifications'), url: '/dashboard/notifications' }];
    this.form = this.fb.group({
      filter: [''],
      type: [[]],
      periority: [[]],
    });

    this.route.queryParams.subscribe((p) => {
      this.page = p['page'] ?? 1;
      this.perPage = p['perPage'] ?? 10;

      this.getList();
    });
  }

  ngOnInit(): void {}

  public get NotificationPeriorityEnum(): typeof NotificationPeriorityEnum {
    return NotificationPeriorityEnum;
  }
  public get NotificationTypeEnum(): typeof NotificationTypeEnum {
    return NotificationTypeEnum;
  }
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
    const input = new GetAllNotificationQuery();
    input.filter = formValue.filter;
    input.type = formValue.type;
    input.periority = formValue.periority;

    input.sortBy = ev ? ev.field : '';
    input.sortDirection = ev ? ev.direction : '';
    this.notificationService
      .getAllNotifications(input)
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
    this.router.navigate(['/dashboard/notifications'], {
      queryParams: {
        page: this.page,
      },
    });
  }

  remove(item: NotificationDto) {
    this.confirmMessage(`${this.l('Delete')}:${item?.title}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.notificationService
          .deleteNotification(new DeleteNotificationCommand({ id: item.id }))
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
  view(item: NotificationDto) {
    this.matDialog
      .open(NotificationInfoComponent, {
        data: item,
      })
      .afterClosed()
      .subscribe((r) => {
        item.read = true;
      });
  }
}
