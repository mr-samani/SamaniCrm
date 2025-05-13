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
import gjsPluginUiImageEditor from 'grapesjs-tui-image-editor';
import gjsPluginTooltip from 'grapesjs-tooltip';

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
      storageManager: {
        autosave: true,
        onStore: (data, editor) => {
          data['produtId'] = 'ddddddddddwwwwwwwerrr';
          // console.log(data);
          return data;
        },
        stepsBeforeSave: 1,
        type: undefined,
      },
      plugins: [
        gjsPresetWebpage,
        gjsPluginForms,
        gjsPluginFlexBox,
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
      }
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
        const data = editor.getProjectData();
        const html = editor.getHtml({ cleanId: true });
        const css = editor.getCss();
        const js = editor.getJs ? editor.getJs() : '';
        console.log('HTML:', html);
        console.log('CSS:', css);
        console.log('JS:', js);
        console.log('data:', JSON.stringify(data));
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
    this.editor.loadData({
      assets: [],
      styles: [
        { selectors: ['countdown'], style: { 'text-align': 'center' }, group: 'cmp:countdown' },
        {
          selectors: ['countdown-block'],
          style: {
            display: 'inline-block',
            'margin-top': '0px',
            'margin-right': '10px',
            'margin-bottom': '0px',
            'margin-left': '10px',
            'padding-top': '10px',
            'padding-right': '10px',
            'padding-bottom': '10px',
            'padding-left': '10px',
          },
          group: 'cmp:countdown',
        },
        { selectors: ['countdown-digit'], style: { 'font-size': '5rem' }, group: 'cmp:countdown' },
        { selectors: ['countdown-endtext'], style: { 'font-size': '5rem' }, group: 'cmp:countdown' },
        { selectors: ['countdown-cont'], style: { display: 'inline-block' }, group: 'cmp:countdown' },
      ],
      pages: [
        {
          frames: [
            {
              component: {
                type: 'wrapper',
                stylable: [
                  'background',
                  'background-color',
                  'background-image',
                  'background-repeat',
                  'background-attachment',
                  'background-position',
                  'background-size',
                ],
                components: [
                  {
                    type: 'countdown',
                    classes: ['countdown'],
                    attributes: { id: 'i3fw' },
                    startfrom: '2025-05-14',
                    components: [
                      {
                        tagName: 'span',
                        classes: ['countdown-cont'],
                        attributes: { 'data-js': 'countdown' },
                        components: [
                          {
                            classes: ['countdown-block'],
                            components: [
                              { classes: ['countdown-digit'], attributes: { 'data-js': 'countdown-day' } },
                              {
                                type: 'text',
                                classes: ['countdown-label'],
                                components: [{ type: 'textnode', content: 'days' }],
                              },
                            ],
                          },
                          {
                            classes: ['countdown-block'],
                            components: [
                              { classes: ['countdown-digit'], attributes: { 'data-js': 'countdown-hour' } },
                              {
                                type: 'text',
                                classes: ['countdown-label'],
                                components: [{ type: 'textnode', content: 'hours' }],
                              },
                            ],
                          },
                          {
                            classes: ['countdown-block'],
                            components: [
                              { classes: ['countdown-digit'], attributes: { 'data-js': 'countdown-minute' } },
                              {
                                type: 'text',
                                classes: ['countdown-label'],
                                components: [{ type: 'textnode', content: 'minutes' }],
                              },
                            ],
                          },
                          {
                            classes: ['countdown-block'],
                            components: [
                              { classes: ['countdown-digit'], attributes: { 'data-js': 'countdown-second' } },
                              {
                                type: 'text',
                                classes: ['countdown-label'],
                                components: [{ type: 'textnode', content: 'seconds' }],
                              },
                            ],
                          },
                        ],
                      },
                      {
                        tagName: 'span',
                        classes: ['countdown-endtext'],
                        attributes: { 'data-js': 'countdown-endtext' },
                      },
                    ],
                  },
                ],
                head: { type: 'head' },
                docEl: { tagName: 'html' },
              },
              id: 'B4hrOu9yjyvYU83M',
            },
          ],
          type: 'main',
          id: 'oL1y3aC7gUCdmwO6',
        },
      ],
      symbols: [],
      dataSources: [],
    });
  }
}
