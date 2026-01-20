import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { finalize, Subscription } from 'rxjs';
import { PageModel } from '../models/page';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { Apis } from '@shared/apis';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import {
  DeletePageCommand,
  GetFilteredPagesQuery,
  PageDto,
  PagesServiceProxy,
  PageTypeEnum,
} from '@shared/service-proxies';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { CreateOrEditPageMetaDataDialogComponent } from '../create-or-edit-page-meta-data-dialog/create-or-edit-page-meta-data-dialog.component';
import { AppConst } from '@shared/app-const';

export class PageDtoExtended extends PageDto {
  statusText?: string;
}

@Component({
  selector: 'pages',
  templateUrl: './pages.component.html',
  styleUrl: './pages.component.scss',
  standalone: false,
})
export class PagesComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: PageDtoExtended[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'title', title: this.l('Title') },
    { column: 'introduction', title: this.l('Introduction') },
    { column: 'description', title: this.l('AdminDescription') },
    { column: 'author', title: this.l('Author') },
    { column: 'created', title: this.l('CreationTime'), type: 'dateTime' },
    { column: 'statusText', title: this.l('Status') },
  ];

  form: FormGroup;
  perPage = AppConst.defaultTablePerPage;
  page = 1;
  listSubscription$?: Subscription;
  showFilter = false;
  type: PageTypeEnum = PageTypeEnum.HomePage;
  constructor(
    injector: Injector,
    private pageService: PagesServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);
    this.form = this.fb.group({
      filter: [''],
    });
    this.page = this.route.snapshot.queryParams['page'] ?? 1;
    this.perPage = this.route.snapshot.queryParams['perPage'] ?? 10;
    this.route.params.subscribe((p) => {
      const typeString = p['type'];
      const type = Object.entries(PageTypeEnum).findIndex((x) => x[1] == typeString);
      if (type == -1) {
        this.router.navigate(['/not-found']);
        return;
      }
      this.type = type;
      this.reload();
    });
  }

  ngOnInit(): void {}

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
    input.type = this.type;
    input.title = this.form.get('filter')?.value;
    input.pageNumber = this.page;
    input.pageSize = this.perPage;
    this.listSubscription$ = this.pageService
      .getAllPages(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data?.items ?? [];
        for (let item of this.list) {
          item.statusText = this.l('PageStatusEnum_') + item.status;
        }
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
    this.router.navigate(['/panel/content/pages/' + PageTypeEnum[this.type]], {
      queryParams: {
        page: this.page,
      },
    });
  }

  openCreatePageDialog() {
    this.matDialog
      .open(CreateOrEditPageMetaDataDialogComponent, {
        data: {
          type: this.type,
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

  updatePageOld(item: PageDtoExtended) {
    this.router.navigate(['/panel/builder/' + item.id]);
  }
  updatePage2(item: PageDtoExtended) {
    this.router.navigate(['/panel/page-builder/' + item.id]);
  }
  updatePage(item: PageDtoExtended) {
    this.router.navigate(['/panel/content/edit/' + item.id]);
  }
  remove(item: PageDtoExtended) {
    this.confirmMessage(`${this.l('Delete')}:${item?.title}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        const input = new DeletePageCommand({
          pageId: item.id,
        });
        this.pageService
          .deletePage(input)
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
