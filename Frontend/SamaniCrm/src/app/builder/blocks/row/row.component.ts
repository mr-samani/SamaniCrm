import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { BlockBase } from '../block-base';
import { BlockDefinition, BlockTypeEnum } from '../block-registry';
import { BlockGeneralHtmlTagsComponent } from '../general-html-tags/general-html-tags.component';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';

@Component({
  selector: 'block-row',
  standalone: true,
  imports: [CommonModule, DynamicRendererComponent, NgxDragDropKitModule],
  templateUrl: './row.component.html',
  styleUrl: './row.component.scss',
})
export class BlockRowComponent extends BlockBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
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
