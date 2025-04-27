import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { finalize } from 'rxjs';
import { PageModel } from '../models/page';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { Apis } from '@shared/apis';
import { FileManagerService } from '@app/file-manager/file-manager.service';

@Component({
  selector: 'pages',
  templateUrl: './pages.component.html',
  styleUrl: './pages.component.scss',
})
export class PagesComponent extends AppComponentBase implements OnInit {
  loading = true;

  list: PageModel[] = [];
  totalCount = 0;

  fields: FieldsType[] = [
    { column: 'cover', title: this.l('Cover'), width: 100, type: 'image' },
    // { column: 'id', title: this.l('Id'), width: 100 },
    { column: 'title', title: this.l('Title') },
    { column: 'description', title: this.l('Description') },
    { column: 'creator', title: this.l('Creator') },
    { column: 'status', title: this.l('Status'), width: 70 },
    { column: 'active', title: this.l('Active'), width: 70 },
    { column: 'createdAt', title: this.l('CreationTime'), type: 'dateTime' },
    { column: 'updatedAt', title: this.l('UpdateTime'), type: 'dateTime' },
  ];

  constructor(
    injector: Injector,
    private fileManagerService: FileManagerService,
  ) {
    super(injector);
    this.breadcrumb.list = [
      { name: this.l('Content'), url: '/dashboard/content' },
      { name: this.l('Pages'), url: '/dashboard/pages' },
    ];
  }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    // this.loading = true;
    // this.dataService
    //   .get<any, PageModel[]>(Apis.pageList, {})
    //   .pipe(finalize(() => (this.loading = false)))
    //   .subscribe((response) => {
    //     this.list = response.data ?? [];
    //     this.totalCount = response.meta!.total;
    //   });
  }

  reload() {
    this.getList();
  }

  changeCover(item: PageModel) {
    this.fileManagerService
      .openFileManager({
        type: 'Image',
        showPreview: true,
      })
      .then((r) => {
        if (r) {
          item.loading = true;
          // this.dataService
          //   .post(Apis.setPageCover, {
          //     id: item.id,
          //     fileId: r,
          //   })
          //   .pipe(finalize(() => (item.loading = false)))
          //   .subscribe((response) => {
          //     this.reload();
          //   });
        }
      });
  }

  openPageDialog(item?: PageModel) {}
}
