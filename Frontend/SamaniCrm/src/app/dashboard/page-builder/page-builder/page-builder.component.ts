import { AfterViewInit, Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import grapesjs, { Editor, Properties } from 'grapesjs';
import gjsPresetWebpage from 'grapesjs-preset-webpage';
import gjsPluginForms from 'grapesjs-plugin-forms';
import gjsPluginFlexBox from 'grapesjs-blocks-flexbox';
import gjsPluginBlocksBasic from 'grapesjs-blocks-basic';
import gjsPluginCountDown from 'grapesjs-component-countdown';
import gjsPluginStyleBg from 'grapesjs-style-bg';
import gjsPluginStyleGradient from 'grapesjs-style-gradient';
import gjsPluginTyped from 'grapesjs-typed';

@Component({
  selector: 'app-page-builder',
  templateUrl: './page-builder.component.html',
  styleUrls: ['./page-builder.component.scss'],
  standalone: false,
})
export class PageBuilderComponent extends AppComponentBase implements AfterViewInit, OnInit, OnDestroy {
  editor?: Editor;
  constructor(injector: Injector) {
    super(injector);
    this.dashboardService.showBreadCrumb = false;
  }
  ngOnInit(): void {}
  ngOnDestroy(): void {
    this.dashboardService.showBreadCrumb = true;
  }
  ngAfterViewInit() {
    this.editor = grapesjs.init({
      container: '#editor',
      height: '100vh',
      width: 'auto',
      fromElement: false,
      storageManager: false, // غیرفعال کردن ذخیره‌سازی پیش‌فرض
      plugins: [
        gjsPresetWebpage,
        gjsPluginForms,
        gjsPluginFlexBox,
        gjsPluginBlocksBasic,
        gjsPluginCountDown,
        gjsPluginStyleBg,
        gjsPluginStyleGradient,
        gjsPluginTyped,
      ],
      pluginsOpts: {
        ['grapesjs-preset-webpage']: {
          modalImportTitle: 'Paste your code here',
          modalImportLabel: '',
          modalImportContent: (editor: Editor) => {
            return editor.getHtml();
          },
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
    this.editor.Panels.addButton('options', [
      {
        id: 'save-page',
        label: '<i class="fa fa-save"></i> ذخیره',
        command: 'save-page',
        attributes: { title: 'ذخیره صفحه' },
      },
      {
        id: 'preview-custom',
        label: '<i class="fa fa-eye"></i> پیش‌نمایش',
        command: 'custom-preview',
        attributes: { title: 'پیش‌نمایش' },
      },
    ]);

    // دستور (command) برای پیش‌نمایش
    this.editor.Commands.add('custom-preview', {
      run(editor) {
        editor.runCommand('preview'); // استفاده از command داخلی
      },
    });

    // دستور (command) برای ذخیره
    this.editor.Commands.add('save-page', {
      run(editor) {
        const html = editor.getHtml();
        const css = editor.getCss();
        const js = editor.getJs ? editor.getJs() : '';
        console.log('HTML:', html);
        console.log('CSS:', css);
        console.log('JS:', js);
        // ذخیره‌سازی دلخواه (ارسال به API، ذخیره محلی و ...)
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

      fontProp.set('defaults', 'iranSans'); // فونت پیش‌فرض
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
    this.editor.onReady(() => this.loadData());
  }

  loadData() {
    if (!this.editor) return;
    this.editor.setComponents(`
  <section class="bdg-sect">
    <h1 class="myheading">متن دلخواه ببببببب
    </h1>
    <p class="paragraph">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua
    </p>
  </section>
  <script>
      alert('messages');
  </script>
`);
    this.editor.setStyle(`
.myheading{float:none;text-align:center;text-shadow:rgb(236, 10, 10) 0px 0px 6px;color:rgb(0, 33, 159);font-family:iranSans;font-size:2em;font-weight:900;border-top-width:2px;border-right-width:2px;border-bottom-width:2px;border-left-width:2px;border-top-style:dotted;border-right-style:dotted;border-bottom-style:dotted;border-left-style:dotted;border-top-color:rgb(224, 124, 0);border-right-color:rgb(224, 124, 0);border-bottom-color:rgb(224, 124, 0);border-left-color:rgb(224, 124, 0);border-image-source:initial;border-image-slice:initial;border-image-width:initial;border-image-outset:initial;border-image-repeat:initial;background-color:#c7c7c7;border-radius:20px 20px 20px 20px;max-width:50%;margin:3em auto 0px auto;}
`);
  }
}
