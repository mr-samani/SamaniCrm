import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { AppComponentBase } from '@app/app-component-base';
import { GridLayoutModule, IGridLayoutOptions, NgxDragDropKitModule } from 'ngx-drag-drop-kit';

@Component({
  selector: 'app-grid-generator',
  templateUrl: './grid-generator.component.html',
  styleUrls: ['./grid-generator.component.scss'],
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule, GridLayoutModule],
})
export class GridGeneratorComponent extends AppComponentBase implements OnInit {
  options: IGridLayoutOptions = {
    cols: 12,
    gap: 10,
    //pushOnDrag: false,
    gridBackgroundConfig: {
      borderWidth: 1,
    },
  };
  layouts: DashboardItem[] = [
    { id: '1', config: new GridItemConfig(3, 0, 2, 3) },
    { id: '2', config: new GridItemConfig(5, 3, 2, 4) },
    { id: '3', config: new GridItemConfig(0, 6, 2, 2) },
    { id: '4', config: new GridItemConfig(0, 7, 1, 1) },
    { id: '5', config: new GridItemConfig(9, 3, 2, 10) },
    { id: '6', config: new GridItemConfig(0, 12, 3, 3) },
  ];

  code: { html: any; style: string } = { html: '', style: '' };

  constructor(
    injector: Injector,
    private domSanitizer: DomSanitizer,
  ) {
    super(injector);
  }

  ngOnInit() {}

  ok() {
    this.code = generateGridLayoutV2(this.layouts, this.options.cols);
    console.log(this.code);
    this.code.html = this.domSanitizer.bypassSecurityTrustHtml(this.code.html);
  }

  onLayoutChange(ev: any[]) {
    let l = ev.map((m) => {
      return {
        id: m.id,
        config: m,
      };
    });
    this.code = generateGridLayoutV2(l, this.options.cols);
    console.log(this.code);
    this.code.html = this.domSanitizer.bypassSecurityTrustHtml(this.code.html);
  }
}

export interface DashboardItem {
  config: GridItemConfig;
  id?: string;
}
export class GridItemConfig {
  x: number;
  y: number;
  w: number;
  h: number;
  constructor(x: number = 0, y: number = 0, w: number = 1, h: number = 1) {
    this.x = x;
    this.y = y;
    this.w = w;
    this.h = h;
  }
}
function generateGridLayout(layout: DashboardItem[], totalColumns = 12): { html: string; style: string } {
  // Determine max rows
  const maxRow = layout.reduce((max, item) => Math.max(max, item.config.y + item.config.h), 0);
  const containerStyle = `
    display: grid;
    grid-template-columns: repeat(${totalColumns}, 1fr);
    grid-auto-rows: 50px;
    gap: 10px;
  `.trim();

  const html = layout
    .map((item) => {
      const style = `
      grid-column: ${item.config.x + 1} / span ${item.config.w};
      grid-row: ${item.config.y + 1} / span ${item.config.h};
      background: #eee;
      border: 1px solid #ccc;
      display: flex;
      align-items: center;
      justify-content: center;
    `.trim();

      return `<div style="${style}">Item ${item.id}</div>`;
    })
    .join('\n');

  return {
    style: containerStyle,
    html: `<div style="${containerStyle}">\n${html}\n</div>`,
  };
}

function generateGridLayoutV2(layouts: DashboardItem[], totalColumns = 12) {
  const containerStyle = `
    display: grid;
    grid-template-columns: repeat(${totalColumns}, 1fr);
    grid-auto-rows: 50px;
    gap: 10px;
  `.trim();

  // Sort by Y then X to preserve visual stacking order
  const sortedLayouts = [...layouts].sort((a, b) => {
    if (a.config.y === b.config.y) return a.config.x - b.config.x;
    return a.config.y - b.config.y;
  });

  const html = sortedLayouts
    .map((item) => {
      const { x, y, w, h } = item.config;
      const style = `
      grid-column: ${x + 1} / span ${w};
      grid-row: ${y + 1} / span ${h};
      background: #eef; color:red;
      border: 1px solid #99c;
      display: flex;
      align-items: center;
      justify-content: center;
    `.trim();

      return `<div style="${style}">Item ${item.id}</div>`;
    })
    .join('\n');

  return {
    style: containerStyle,
    html: `<div style="${containerStyle}">\n${html}\n</div>`,
  };
}
