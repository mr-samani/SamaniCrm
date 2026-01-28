import { AfterViewInit, ChangeDetectorRef, Component,  OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { finalize } from 'rxjs/operators';
import {
  IPage,
  NGX_PAGE_BUILDER_FILE_PICKER,
  NGX_PAGE_BUILDER_HTML_EDITOR,
  NgxPageBuilder,
  NGX_PAGE_BUILDER_STORAGE_SERVICE,
  IStyleSheetFile,
} from 'ngx-page-builder';
import { PageStorageService } from '../page-builder/page-storage.service';
import { FilePickerService } from '../page-builder/file-picker.service';
import { HtmlEditorService } from '../page-builder/html-editor.service';
import { SharedPageDataService } from '../page-builder/shared-page-data.service';

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
  previousMainHeaderFixedTop: boolean = AppConst.mainHeaderFixedTop;

  pageId = '';
  data: IPage[] = [];
  styles: IStyleSheetFile[] = [];

  @ViewChild('pageBuilder') pageBuilder!: NgxPageBuilder;
  constructor(
    private pageService: PagesServiceProxy,
    private cd: ChangeDetectorRef,
    public sharedPageDataService: SharedPageDataService,
  ) {
    super();
    this.pageId = this.route.snapshot.params['id'];
  }

  ngOnInit() {
    this.getPageInfo();
  }
  ngAfterViewInit(): void {
    setTimeout(() => {
      AppConst.mainHeaderFixedTop = false;
      this.cd.detectChanges();
      this.doc.querySelector('ngx-page-builder')?.scrollIntoView();
    });
  }
  ngOnDestroy(): void {
    AppConst.mainHeaderFixedTop = this.previousMainHeaderFixedTop;
  }
  getPageInfo() {
    if (!this.pageId) return;
    this.showMainLoading();
    this.pageService
      .getPageInfo(this.pageId, AppConst.currentLanguage)
      .pipe(finalize(() => this.hideMainLoading()))
      .subscribe((response) => {
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
