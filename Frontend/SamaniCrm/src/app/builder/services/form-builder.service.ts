import { Injectable } from '@angular/core';
import { BlockTypeEnum, BLOCK_REGISTRY, BlockDefinition, FormTools, BlockData } from '../blocks/block-registry';
import { IDropEvent, moveItemInArray } from 'ngx-drag-drop-kit';
import { ViewModeEnum } from '../models/view-mode.enum';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { createTreeFormTools } from '../helpers/tools';
import { PageBuilderServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { CanChildHtmlTags, SimpleHtmlTags } from '../blocks/general-html-tags/GeneralTagNames';
import { cloneDeep } from 'lodash-es';
import { generateSequentialGuid } from '@shared/helper/guid';
import { DynamicDataService } from './dynamic-data.service';
import { BlockGeneralHtmlTagsComponent } from '../blocks/general-html-tags/general-html-tags.component';
import { objectToStyle } from '../properties/styles/helper/objectToStyle';
import { findNearestDynamicDataCacheKey } from '../properties/styles/helper/findNearestDynamicDataCacheKey';
import { HistoryService } from './history.service';

@Injectable()
export class FormBuilderService {
  blocks: BlockDefinition[] = [];
  tools: FormTools[] = [];
  viewMode = ViewModeEnum.Desktop;

  selectedBlock?: BlockDefinition;

  showBorder = true;

  /**
   * show layput or tools
   * - false: show tools
   * - true: show layouts
   */
  showLayouts = false;

  loadingCustomBlocks = true;

  constructor(
    private alert: NgxAlertModalService,
    private pageBuilderService: PageBuilderServiceProxy,
    private history: HistoryService,
    public ds: DynamicDataService,
  ) {}

  public cleanServiceData() {
    this.blocks = [];
    this.tools = [];
    this.viewMode = ViewModeEnum.Desktop;
    this.selectedBlock = undefined;
    this.showBorder = true;
    this.showLayouts = false;
    this.loadingCustomBlocks = true;
    this.ds.reset();
    this.history.clear();
  }

  getCustomBlocks() {
    // کامپوننت های سفارشی تعریف شده
    const advancedBlocks = BLOCK_REGISTRY;
    // کامپوننت عمومی با تگ هایی که می توانند فرزند داشته باشند
    const generalTags = CanChildHtmlTags.map((m) => {
      return <BlockDefinition>{
        category: 'Container',
        type: BlockTypeEnum.GeneralHtmlTag,
        component: BlockGeneralHtmlTagsComponent,
        canChild: true,
        tagName: m,
        name: m,
        icon: 'fa block-' + m,
      };
    });
    // کاموننت های ساده مانند span,u,i,b,...
    const simpleTags = SimpleHtmlTags.map((m) => {
      return <BlockDefinition>{
        category: 'General',
        type: BlockTypeEnum.GeneralHtmlTag,
        component: BlockGeneralHtmlTagsComponent,
        canChild: false,
        tagName: m,
        name: m,
        icon: 'fa block-' + m,
        data: {
          text: 'This is a ' + m,
        },
      };
    });
    this.tools = createTreeFormTools([...advancedBlocks, ...generalTags, ...simpleTags]);

    this.loadingCustomBlocks = true;
    this.pageBuilderService
      .getCustomBlocks()
      .pipe(finalize(() => (this.loadingCustomBlocks = false)))
      .subscribe((result) => {
        const list = result.data ?? [];
        const customBlocks: BlockDefinition[] = [];
        for (let item of list) {
          if (item.data) {
            const b = JSON.parse(item.data ?? '{}');
            b.isCustomBlock = true;
            b.canDelete = item.canDelete;
            b.name = item.name;
            b.id = item.id;
            b.description = item.description;
            b.icon = item.icon;
            b.image = item.image;
            customBlocks.push(b);
          }
        }
        const blocks = createTreeFormTools(customBlocks);
        // console.log('customBlocks', blocks);
        this.tools.push(...blocks);
      });
  }

  addBlock(source: BlockDefinition, parent?: BlockDefinition, index?: number, blocks?: BlockDefinition[]) {
    if (!blocks) blocks = this.blocks;
    if (index == undefined) {
      index = blocks.length;
    }
    let s = cloneDeep(source);
    // important create new id
    s.id = generateSequentialGuid();
    s.data ??= new BlockData();
    s.parent = parent;
    if (parent) s.dynamicDataCacheKey = findNearestDynamicDataCacheKey(parent);
    s = new BlockDefinition(s);
    blocks.splice(index, 0, s);
    this.updateRowNumber(this.blocks);
    this.history.save('add', s, `Add block'${s.id}' to '${parent?.id}'`);
  }

  drop(event: IDropEvent<BlockDefinition[]>, parent?: BlockDefinition) {
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
        this.addBlock(item, parent, event.currentIndex, event.container.data);
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

  deleteBlock(block: BlockDefinition, parent?: BlockDefinition) {
    this.alert
      .show({
        title: 'Delete: ' + BlockTypeEnum[block.type],
        text: 'Are you sure delete?',
        showConfirmButton: true,
        showCancelButton: true,
      })
      .then((r) => {
        if (r.isConfirmed) {
          this.history.save('delete', block, `Delete block '${block.id}' from '${parent?.id}'`);
          if (!parent) {
            const foundedIndex = this.blocks.findIndex((x) => x.id == block.id);
            if (foundedIndex > -1) {
              this.blocks.splice(foundedIndex, 1);
            }
          } else {
            if (parent.children) {
              const foundedIndex = parent.children.findIndex((x) => x.id == block.id);
              if (foundedIndex > -1) {
                parent.children?.splice(foundedIndex, 1);
              }
            }
          }
          this.selectedBlock = undefined;
        }
      });
  }

  updateCss(block?: BlockDefinition, setHistory = true) {
    if (!block) {
      block = this.selectedBlock;
    }
    if (!block) return;

    if (!block.data) {
      block.data = new BlockData();
    } 
    // console.log(block.data.style)
    block.data.css = objectToStyle(block.data.style);
    //  block.data.css = block.data.css.replace(/\s/g, '');
    if (setHistory) this.history.save('edit', block, `Update style block '${block.id}': '${block.data.css}'`);
  }
}

export class BlockRowData {
  cols: number = 2;
}

export class BlockProductCategoryData {
  title?: string;
  subTitle?: string;
}
