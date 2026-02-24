import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { finalize } from 'rxjs/operators';

import { PageStorageService } from '../services/page-storage.service';
import { FilePickerService } from '../services/file-picker.service';
import { HtmlEditorService } from '../services/html-editor.service';
import { SharedPageDataService } from '../services/shared-page-data.service';
import {
  NGX_PAGE_BUILDER_EXPORT_PLUGIN_STORE,
  NGX_PAGE_BUILDER_FILE_PICKER,
  NGX_PAGE_BUILDER_HTML_EDITOR,
  NGX_PAGE_BUILDER_STORAGE_SERVICE,
  NgxPageBuilder,
  providePageBuilder,
} from 'ngx-page-builder/designer';
import { CustomToolbarButtons, DynamicDataStructure, IPage, IStyleSheetFile } from 'ngx-page-builder/core';
import { PluginService } from '../services/plugin.service';
import { CUSTOM_BLOCKS } from '../custom-blocks/CustomBlocks';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { CommonModule } from '@angular/common';
import { PageBuilderServiceProxy } from '@shared/service-proxies';
import { DYNAMIC_DATA } from '../dynamic-data/dynamic-data'; 
@Component({
  selector: 'app-edit-page',
  templateUrl: './edit-page.component.html',
  styleUrls: ['./edit-page.component.scss'],
  imports: [NgxPageBuilder, CommonModule, ],
  providers: [
    PagesServiceProxy,
    PageBuilderServiceProxy,
    FileManagerService,
    PluginService,
    providePageBuilder({
      customSources: CUSTOM_BLOCKS,
      enableHistory: false,
      enableExportAsPlugin: true,
      showPlugins: true,
      publicCss: ['/bootstrap/bootstrap.min.css', '/swiper/swiper-bundle.min.css'],
      publicJs: ['/bootstrap/bootstrap.min.js'],
      //  storageType: StorageType.None,
      toolbarConfig: {
        showSaveButton: true,
        showConfigButton: false,
        showExportHtmlButton: false,
        showImportHtmlButton: true,
        showOpenButton: false,
        showPreviewButton: true,
        showPrintButton: true,
      },
    }),
    {
      provide: NGX_PAGE_BUILDER_EXPORT_PLUGIN_STORE,
      useExisting: PluginService,
      deps: [],
    },
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
          const c = this.lang.substring(0, 2);
          window.open(
            `${AppConst.publicSiteUrl}/${c}/page/preview/${this.sharedPageDataService.pageInfo.culture}/${this.pageId}`,
            '_blank',
          );
        }
      },
    },
  ];

  dynamicData: DynamicDataStructure[] = DYNAMIC_DATA;
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
    this.pageService
      .getPageInfo(this.pageId, this.lang)
      .pipe(
        finalize(() => {
          this.hideMainLoading();
          this.chdr.detectChanges();
        }),
      )
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
