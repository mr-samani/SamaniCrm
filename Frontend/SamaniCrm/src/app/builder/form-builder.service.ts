import { Injectable } from '@angular/core';
import { BlockTypeEnum, BLOCK_REGISTRY, BlockDefinition } from './blocks/block-registry';
import { BlockDivComponent } from './blocks/div/div.component';
import { IDropEvent, moveItemInArray } from 'ngx-drag-drop-kit';
import { ViewModeEnum } from './models/view-mode.enum';

@Injectable()
export class FormBuilderService {
  blocks: BlockDefinition[] = [];
  blocksList = BLOCK_REGISTRY;
  viewMode = ViewModeEnum.Desktop;

  selectedBlock?: BlockDefinition;

  showBorder = true;

  constructor() {}

  addBlock(type: BlockTypeEnum, index?: number, parentChildren?: BlockDefinition[]) {
    if (!parentChildren) parentChildren = this.blocks;
    if (index == undefined) {
      index = parentChildren.length;
    }
    const def = BLOCK_REGISTRY.find((b) => b.type === type);
    if (def) {
      let b = new BlockDefinition({ type: def.type, data: def.data });
      if (b.type == BlockTypeEnum.Row && (!b.children || b.children.length < 1)) {
        // هر Row باید دو cell (Div) داشته باشد که هرکدام children آرایه‌ای خالی دارند
        b.children = [
          new BlockDefinition({ type: BlockTypeEnum.Div, component: BlockDivComponent, children: [] }),
          new BlockDefinition({ type: BlockTypeEnum.Div, component: BlockDivComponent, children: [] }),
        ];
      }
      parentChildren.splice(index, 0, b);
    }
    this.updateRowNumber(this.blocks);
  }

  drop(event: IDropEvent<BlockDefinition[]>) {
    // اگر در همان cell جابجا شد
    if (event.previousContainer === event.container) {
      if (event.container.data) {
        moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      }
    } else {
      // جابجایی بین cellها
      if (event.previousContainer.data && event.container.data) {
        const item = event.previousContainer.data[event.previousIndex];
        if (event.previousContainer.el.id != 'toolBox') {
          event.previousContainer.data.splice(event.previousIndex, 1);
        }
        this.addBlock(item.type, event.currentIndex, event.container.data);
        //event.container.data.splice(event.currentIndex, 0, item);
      }
    }
  }

  private updateRowNumber(list: BlockDefinition[], rowNumber = 1) {
    for (let block of list) {
      block.rowNumber = rowNumber;
      rowNumber++;
      if (block.children) {
        rowNumber = this.updateRowNumber(block.children, rowNumber);
      }
    }
    return rowNumber;
  }

  onSelect(block: BlockDefinition, ev: Event) {
    ev.stopPropagation();
    this.selectedBlock = block;
  }
}

export class BlockRowData {
  cols: number = 2;
}

export class BlockProductCategoryData {
  title?: string;
  subTitle?: string;
}
