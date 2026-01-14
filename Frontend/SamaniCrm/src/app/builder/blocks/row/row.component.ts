import { Component, Injector, OnInit } from '@angular/core';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { BlockBase } from '../block-base';
import { BlockDefinition, BlockTypeEnum } from '../block-registry';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { BlockGeneralHtmlTagsComponent } from '../general-html-tags/general-html-tags.component';

@Component({
  selector: 'block-row',
  standalone: true,
  imports: [DynamicRendererComponent, NgxDragDropKitModule],
  templateUrl: './row.component.html',
  styleUrl: './row.component.scss',
})
export class BlockRowComponent extends BlockBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    if (this.block && (!this.block.children || this.block.children.length < 1)) {
      // هر Row باید دو cell (Div) داشته باشد که هرکدام children آرایه‌ای خالی دارند
      this.block.children = [
        new BlockDefinition({
          type: BlockTypeEnum.GeneralHtmlTag,
          tagName: 'div',
          component: BlockGeneralHtmlTagsComponent,
        }),
        new BlockDefinition({
          type: BlockTypeEnum.GeneralHtmlTag,
          tagName: 'div',
          component: BlockGeneralHtmlTagsComponent,
        }),
      ];
    }
  }
}
