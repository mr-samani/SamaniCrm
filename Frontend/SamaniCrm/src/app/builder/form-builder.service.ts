import { Injectable, signal } from '@angular/core';
import { BlockTypeEnum, BLOCK_REGISTRY, BlockDefinition } from './blocks/block-registry';
import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';

@Injectable()
export class FormBuilderService {
  blocks: BlockDefinition[] = [];
  blocksList = BLOCK_REGISTRY;

  dragTargetIds = signal<string[]>(['canvas', 'toolBox']);

  addBlock(type: BlockTypeEnum, index?: number) {
    if (index == undefined) {
      index = this.blocks.length;
    }
    const def = BLOCK_REGISTRY.find((b) => b.type === type);
    if (def) {
      this.blocks.splice(index, 0, { type: def.type, data: { ...def.defaultData } });
    }

    this.prepareDragDrop();
  }

  prepareDragDrop() {
    let list = ['canvas', 'toolBox'];
    for (let c = 0; c < this.blocks.length; c++) {
      if (this.blocks[c].type == BlockTypeEnum.Row) {
        for (let i = 0; i < (this.blocks[c].data as any).cols; i++) {
          list.push(`cell_${c}_${i}`);
        }
      }
    }
    this.dragTargetIds.set(list);
  }
}

export class BlockRowData {
  cols: number = 2;
}

export class BlockProductCategoryData {
  title?: string;
  subTitle?: string;
}
