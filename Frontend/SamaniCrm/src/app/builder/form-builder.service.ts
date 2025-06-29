import { ChangeDetectorRef, Injectable, signal } from '@angular/core';
import { BlockTypeEnum, BLOCK_REGISTRY, BlockDefinition } from './blocks/block-registry';
import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';
import { BlockDivComponent } from './blocks/div/div.component';

@Injectable()
export class FormBuilderService {
  blocks: BlockDefinition[] = [];
  blocksList = BLOCK_REGISTRY;

  constructor(private cd: ChangeDetectorRef) {
  }

  addBlock(type: BlockTypeEnum, index?: number, parentChildren?: BlockDefinition[]) {
    if (!parentChildren) parentChildren = this.blocks;
    if (index == undefined) {
      index = parentChildren.length;
    }
    const def = BLOCK_REGISTRY.find((b) => b.type === type);
    if (def) {
      let b = new BlockDefinition({ type: def.type, data: { ...def.defaultData } });
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
}

export class BlockRowData {
  cols: number = 2;
}

export class BlockProductCategoryData {
  title?: string;
  subTitle?: string;
}
