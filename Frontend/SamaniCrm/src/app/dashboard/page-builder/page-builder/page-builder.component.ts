import { AfterViewInit, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import grapesjs, { Editor } from 'grapesjs';
import gjsPresetWebpage from 'grapesjs-preset-webpage';
import gjsPluginForms from 'grapesjs-plugin-forms';
import gjsPluginFlexBox from 'grapesjs-blocks-flexbox';
import gjsPluginBlocksBasic from 'grapesjs-blocks-basic';
import gjsPluginCountDown from 'grapesjs-component-countdown';

@Component({
  selector: 'app-page-builder',
  templateUrl: './page-builder.component.html',
  styleUrls: ['./page-builder.component.scss'],
  standalone: false,
})
export class PageBuilderComponent extends AppComponentBase implements AfterViewInit {
  editor?: Editor;
  constructor(injector: Injector) {
    super(injector);
  }

  ngAfterViewInit() {
    this.editor = grapesjs.init({
      container: '#editor',
      fromElement: true,
      height: 'calc(100vh - 150px)',
      width: 'auto',
      storageManager: { autoload: false },
      plugins: [gjsPresetWebpage, gjsPluginBlocksBasic, gjsPluginForms, gjsPluginFlexBox, gjsPluginCountDown],
      pluginsOpts: {
        ['gjsPresetWebpage']: {
          // Optional: تنظیمات پیش‌فرض را override کن
          blocksBasicOpts: { flexGrid: true },
          navbarOpts: {},
          blockManager: {
            appendTo: '#blocks',
          },
        },
      },
    });
  }

  setValue() {
    if (this.editor) {
      this.editor.loadData(`<body id="ic36">
  <section class="bdg-sect">
    <h1 class="heading">متن دلخواه
    </h1>
    <p class="paragraph">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua
    </p>
  </section>
</body>`);
      this.editor.setStyle(`* {
  box-sizing: border-box;
}
body {
  margin: 0;
  direction:rtl;
}
.heading{
  float:none;
  text-align:center;
  text-shadow:0px 0px 6px #ec0a0a;
  color:#00219f;
  font-family:Times New Roman, Times, serif;
  font-size:2em;
  font-weight:200;
  border:2px dotted #e07c00;
}
`);
    }
  }
}
