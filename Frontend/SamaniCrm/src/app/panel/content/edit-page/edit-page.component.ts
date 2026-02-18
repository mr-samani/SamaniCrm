import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { finalize } from 'rxjs/operators';

import { PageStorageService } from '../page-builder/page-storage.service';
import { FilePickerService } from '../page-builder/file-picker.service';
import { HtmlEditorService } from '../page-builder/html-editor.service';
import { SharedPageDataService } from '../page-builder/shared-page-data.service';
import {
  NGX_PAGE_BUILDER_FILE_PICKER,
  NGX_PAGE_BUILDER_HTML_EDITOR,
  NGX_PAGE_BUILDER_STORAGE_SERVICE,
  NgxPageBuilder,
} from 'ngx-page-builder/designer';
import { CustomToolbarButtons, IPage, IStyleSheetFile } from 'ngx-page-builder/core';
@Component({
  selector: 'app-edit-page',
  templateUrl: './edit-page.component.html',
  styleUrls: ['./edit-page.component.scss'],
  imports: [NgxPageBuilder],
  providers: [
    PagesServiceProxy,
    { provide: NGX_PAGE_BUILDER_FILE_PICKER, useClass: FilePickerService },
    { provide: NGX_PAGE_BUILDER_HTML_EDITOR, useClass: HtmlEditorService },
    { provide: NGX_PAGE_BUILDER_STORAGE_SERVICE, useClass: PageStorageService },
  ],
})
export class EditPageComponent extends AppComponentBase implements OnInit, AfterViewInit, OnDestroy {
  pageId = '';
  lang = '';
  data: IPage[] = [];
  styles: IStyleSheetFile[] = [];

  customButtons: CustomToolbarButtons[] = [
    {
      title: 'close',
      icon: '<i class="fa fa-rectangle-xmark"></i>',
      callback: () => {
        debugger;
        if (window.opener) {
          window.opener.postMessage('closed', '*');
        }
        window.close();
      },
    },
    {
      title: 'Preview Page',
      icon: '<i class="fa fa-desktop-arrow-down"></i>',
      callback: () => {
        if (this.sharedPageDataService.pageInfo) {
          const c = AppConst.currentLanguage.substring(0, 2);
          window.open(
            `${AppConst.publicSiteUrl}/${c}/page/preview/${this.sharedPageDataService.pageInfo.culture}/${this.pageId}`,
            '_blank',
          );
        }
      },
    },
  ];
  @ViewChild('pageBuilder') pageBuilder!: NgxPageBuilder;
  constructor(
    private pageService: PagesServiceProxy,
    private cd: ChangeDetectorRef,
    public sharedPageDataService: SharedPageDataService,
  ) {
    super();
    this.lang = this.route.snapshot.params['lang'];
    this.pageId = this.route.snapshot.params['id'];
  }

  ngOnInit() {
    this.getPageInfo();
  }
  ngAfterViewInit(): void {}
  ngOnDestroy(): void {}
  getPageInfo() {
    if (!this.pageId) return;
    this.showMainLoading();
    debugger;
    this.pageService
      .getPageInfo(this.pageId, this.lang)
      .pipe(finalize(() => this.hideMainLoading()))
      .subscribe((response) => {
        console.log(response);
        this.sharedPageDataService.pageInfo = response.data;
        try {
          this.styles = JSON.parse(this.sharedPageDataService.pageInfo?.styles ?? '[]');
        } catch (error) {
          console.warn('Error on parse styles:', error);
          this.styles = [];
        }
        if (this.sharedPageDataService.pageInfo && this.sharedPageDataService.pageInfo.data) {
          const d: IPage[] = JSON.parse(this.sharedPageDataService.pageInfo.data ?? '[]');
          this.data = d ?? [];
        }
      });
  }
}
