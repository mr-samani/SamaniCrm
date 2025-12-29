import { AfterViewInit, Component, Injector, OnDestroy, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import grapesjs, { Editor } from 'grapesjs';
import gjsPresetWebpage from 'grapesjs-preset-webpage';
import gjsPluginForms from 'grapesjs-plugin-forms';
import gjsPluginBlocksBasic from 'grapesjs-blocks-basic';
import gjsPluginCountDown from 'grapesjs-component-countdown';
import gjsPluginStyleBg from 'grapesjs-style-bg';
import gjsPluginStyleGradient from 'grapesjs-style-gradient';
import gjsPluginTyped from 'grapesjs-typed';
import gjsPluginUiImageEditor from 'grapesjs-tui-image-editor';
import gjsPluginTooltip from 'grapesjs-tooltip';
import { PageDto, PagesServiceProxy, UpdatePageContentCommand } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { AppConst } from '@shared/app-const';
import { MatDialog } from '@angular/material/dialog';
import { CreateOrEditPageMetaDataDialogComponent } from '@app/panel/content/create-or-edit-page-meta-data-dialog/create-or-edit-page-meta-data-dialog.component';
import { ProductCategoryElement } from '../blocks/product-category/product-category.element.ts';

@Component({
  selector: 'app-page-builder',
  templateUrl: './page-builder.component.html',
  styleUrls: ['./page-builder.component.scss'],
  standalone: false,
  encapsulation: ViewEncapsulation.None,
})
export class PageBuilderComponent extends AppComponentBase implements AfterViewInit, OnDestroy {
  editor?: Editor;
  pageId: string;
  saving = false;
  loading = true;
  pageInfo?: PageDto;

  constructor(
    injector: Injector,
    private pageService: PagesServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);
    this.panelService.showBreadCrumb = false;
    this.pageId = this.route.snapshot.params['pageId'];
  }

  ngAfterViewInit(): void {
    this.getPageInfo();
  }

  ngOnDestroy(): void {
    this.panelService.showBreadCrumb = true;
  }

  getPageInfo() {
    this.loading = true;
    this.pageService
      .getPageInfo(this.pageId, AppConst.currentLanguage)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.pageInfo = response.data;
        this.setup();
      });
  }

  setup() {
    this.editor = grapesjs.init({
      container: '#editor',
      height: '100vh',
      width: 'auto',
      fromElement: false,
      storageManager: {
        autosave: true,
        onStore: (data, editor) => {
          data['pageId'] = this.pageId;
          return data;
        },
        stepsBeforeSave: 1,
        type: undefined,
      },
      plugins: [
        gjsPresetWebpage,
        gjsPluginForms,
        gjsPluginBlocksBasic,
        gjsPluginCountDown,
        gjsPluginStyleBg,
        gjsPluginStyleGradient,
        gjsPluginTyped,
        gjsPluginUiImageEditor,
        gjsPluginTooltip,
      ],
      pluginsOpts: {
        ['grapesjs-preset-webpage']: {
          modalImportTitle: 'Paste your code here',
          modalImportLabel: '',
          modalImportContent: (editor: Editor) => editor.getHtml(),
          importViewerOptions: {},
          showStylesOnChange: 1,
          textCleanCanvas: 'Are you sure to clean the canvas?',
        },
      },
      canvas: {
        styles: ['/public-style.css'],
        frameContent: '<!DOCTYPE html><body dir="rtl">',
      },
    });

    this.customizeGrapesjs();
  }

  customizeGrapesjs() {
    if (!this.editor) return;
    const self = this;

    this.editor.BlockManager.add('category-list', ProductCategoryElement);

    this.editor.DomComponents.addType('category-list', {
      isComponent: (el) => el.dataset && el.dataset['component'] === 'CategoryList',
      model: {
        defaults: {
          traits: [
            {
              type: 'number',
              name: 'category-id',
              label: 'Category ID',
            },
          ],
        },
      },
    });

    this.editor.Panels.addButton('options', [
      {
        id: 'save-page',
        className: 'fa fa-save',
        //label: '<i class="fa fa-save"></i> ذخیره',
        command: 'save-page',
        attributes: { title: 'ذخیره صفحه' },
      },
      {
        id: 'preview-custom',
        className: 'fa fa-eye',
        command: 'custom-preview',
        attributes: { title: 'پیش‌نمایش' },
      },
      {
        id: 'edit-page-meta-data',
        className: 'fa fa-edit',
        command: 'edit-page-meta-data',
      },
    ]);

    this.editor.Commands.add('custom-preview', {
      run(editor) {
        // editor.runCommand('preview');
        window.open(AppConst.publicSiteUrl + '/page-preview-old/' + self.pageInfo?.culture + '/' + self.pageId, '_blank');
      },
    });

    // دستور (command) برای ذخیره
    this.editor.Commands.add('save-page', {
      run(editor) {
        const data = editor.getProjectData();
        const html = editor.getHtml({ cleanId: true });
        const css = editor.getCss();
        const js = editor.getJs ? editor.getJs() : '';
        self.saving = true;
        const input = new UpdatePageContentCommand({
          pageId: self.pageId,
          culture: self.pageInfo?.culture,
          data: JSON.stringify(data),
          html: html,
          scripts: js,
          styles: css,
        });
        self.pageService
          .updatePageContent(input)
          .pipe(finalize(() => (self.saving = false)))
          .subscribe(() => {
            self.notify.success(self.l('SavedSuccessfully'));
          });
      },
    });

    this.editor.Commands.add('edit-page-meta-data', {
      run(editor) {
        self.matDialog.open(CreateOrEditPageMetaDataDialogComponent, {
          data: {
            id: self.pageId,
            type: self.pageInfo?.type,
          },
        });
      },
    });

    const sm = this.editor.StyleManager;
    const fontProp = sm.getProperty('typography', 'font-family');

    if (fontProp) {
      fontProp.set('options' as any, [
        { id: 'iranSans', label: 'IranSans' },
        { id: 'Arial, Helvetica, sans-serif', label: 'Arial' },
        { id: 'Tahoma, Geneva, sans-serif', label: 'Tahoma' },
        { id: 'Times New Roman, serif', label: 'Times New Roman' },
      ]);
      fontProp.set('defaults', 'iranSans');
    }

    // this.editor.StyleManager.addProperty('typography', {
    //   name: 'Font family',
    //   property: 'font-family',
    //   type: 'select',
    //   defaults: 'iranSans',
    //   options: [
    //     { id: 'iranSans', label: 'IranSans' },
    //     { id: 'Arial, Helvetica, sans-serif', label: 'Arial' },
    //     { id: 'Tahoma, Geneva, sans-serif', label: 'Tahoma' },
    //     { id: 'Times New Roman, serif', label: 'Times New Roman' },
    //   ],
    // });
    this.editor.onReady(() => {
      try {
        let data = JSON.parse(this.pageInfo?.data ?? '');
        this.editor!.loadData(data);
      } catch (error) {
        console.error('Error on parse data:', error);
      }
    });
  }
}
