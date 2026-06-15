import { Component, inject, Injector, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import {
  GetFilteredPagesQuery,
  PageDto,
  PagesServiceProxy,
  PageStatusEnum,
  PageTypeEnum,
  PaginatedResultOfPageDto,
} from '@shared/service-proxies';
import { finalize, Subscription } from 'rxjs';

@Component({
  selector: 'app-select-page-url',
  templateUrl: './select-page-url.component.html',
  styleUrls: ['./select-page-url.component.scss'],
  standalone: false,
})
export class SelectPageUrlComponent extends AppComponentBase implements OnInit {
  loading = false;
  list: PageDto[] = [];
  pageService = inject(PagesServiceProxy);
  dialogRef = inject(MatDialogRef<SelectPageUrlComponent>);

  listSubscription$?: Subscription;
  totalCount = 0;
  perPage = AppConst.defaultTablePerPage;
  page = 1;
  fields: FieldsType[] = [
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'title', title: this.l('Title') },
    { column: 'slug', title: this.l('Slug') },
    { column: 'description', title: this.l('PrivateDescription') },
    { column: 'author', title: this.l('Author') },
    { column: 'created', title: this.l('CreationTime'), type: 'dateTime' },
    { column: 'statusText', title: this.l('Status') },
  ];

  form: FormGroup;
  constructor() {
    super();
    this.form = this.fb.group({
      filter: [''],
    });
  }

  ngOnInit() {
    this.getList();
  }
  ngOnDestroy(): void {
    if (this.listSubscription$) {
      this.listSubscription$.unsubscribe();
    }
  }
  getList() {
    if (this.listSubscription$ && !this.listSubscription$.closed) {
      this.listSubscription$.unsubscribe();
    }
    this.loading = true;
    const input = new GetFilteredPagesQuery();
    input.title = this.form.get('filter')?.value;
    input.status = PageStatusEnum.Published;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    this.listSubscription$ = this.pageService
      .getAllPages(input)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
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
    this.form.patchValue({ filter: '' });
    this.reload();
  }
  onPageChange(ev?: PageEvent) {
    if (ev) this.perPage = ev.perPage;
    this.getList();
  }

  ok(item: PageDto) {
    this.dialogRef.close('page/' + item.slug);
  }
}
