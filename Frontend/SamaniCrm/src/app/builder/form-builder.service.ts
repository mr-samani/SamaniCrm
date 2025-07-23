import { Injectable } from '@angular/core';
import { BlockTypeEnum, BLOCK_REGISTRY, BlockDefinition, FormTools, BlockData } from './blocks/block-registry';
import { BlockGeneralHtmlTagsComponent } from './blocks/general-html-tags/general-html-tags.component';
import { IDropEvent, moveItemInArray } from 'ngx-drag-drop-kit';
import { ViewModeEnum } from './models/view-mode.enum';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { createTreeFormTools } from './helpers/tools';
import { PageBuilderServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { CanChildHtmlTags, SimpleHtmlTags } from './blocks/general-html-tags/GeneralTagNames';
import { cloneDeep } from 'lodash-es';

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
  ) {
    this.getCustomBlocks();
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

  addBlock(source: BlockDefinition, index?: number, parentChildren?: BlockDefinition[]) {
    if (!parentChildren) parentChildren = this.blocks;
    if (index == undefined) {
      index = parentChildren.length;
    }
    const s = cloneDeep(source);
    s.data ??= new BlockData();
    parentChildren.splice(index, 0, s);

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
        this.addBlock(item, event.currentIndex, event.container.data);
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
          if (!parent) {
            const foundedIndex = this.blocks.findIndex((x) => x == block);
            if (foundedIndex > -1) {
              this.blocks.splice(foundedIndex, 1);
            }
          } else {
            const foundedIndex = parent.children?.findIndex((x) => x == block);
            if (foundedIndex && foundedIndex > -1) {
              parent.children?.splice(foundedIndex, 1);
            }
          }
          this.selectedBlock = undefined;
        }
      });
  }

  updateCss() {
    if (!this.selectedBlock) return;
    if (!this.selectedBlock.data) {
      this.selectedBlock.data = new BlockData();
    }
    this.selectedBlock.data.css = `
  ${this.selectedBlock.data.style.border ? 'border:' + this.selectedBlock.data.style.border + ';' : ''}
  ${this.selectedBlock.data.style.padding ? 'padding:' + this.selectedBlock.data.style.padding + ';' : ''}
  ${this.selectedBlock.data.style.margin ? 'margin:' + this.selectedBlock.data.style.margin + ';' : ''}
  ${this.selectedBlock.data.style.borderRadius ? 'border-radius:' + this.selectedBlock.data.style.borderRadius + ';' : ''}
  ${this.selectedBlock.data.style.boxShadow ? 'box-shadow:' + this.selectedBlock.data.style.boxShadow + ';' : ''}
  ${this.selectedBlock.data.style.backgroundColor ? 'background-color:' + this.selectedBlock.data.style.backgroundColor + ';' : ''}
  ${this.selectedBlock.data.style.backgroundImage ? 'background-image:' + this.selectedBlock.data.style.backgroundImage + ';' : ''}
  ${this.selectedBlock.data.style.backgroundSize ? 'background-size:' + this.selectedBlock.data.style.backgroundSize + ';' : ''}
  ${this.selectedBlock.data.style.backgroundRepeat ? 'background-repeat:' + this.selectedBlock.data.style.backgroundRepeat + ';' : ''}`;

    /* dimensions */
    this.selectedBlock.data.css += `
  ${this.selectedBlock.data.style.width ? 'width:' + this.selectedBlock.data.style.width + 'px;' : ''}
  ${this.selectedBlock.data.style.height ? 'height:' + this.selectedBlock.data.style.height + 'px;' : ''}
  ${this.selectedBlock.data.style.minWidth ? 'min-width:' + this.selectedBlock.data.style.minWidth + 'px;' : ''}
  ${this.selectedBlock.data.style.minHeight ? 'min-height:' + this.selectedBlock.data.style.minHeight + 'px;' : ''}
  ${this.selectedBlock.data.style.maxWidth ? 'max-width:' + this.selectedBlock.data.style.maxWidth + 'px;' : ''}
  ${this.selectedBlock.data.style.maxHeight ? 'max-height:' + this.selectedBlock.data.style.maxHeight + 'px;' : ''}`;

    /* position */
    this.selectedBlock.data.css += `
  ${this.selectedBlock.data.style.position ? 'position:' + this.selectedBlock.data.style.position + ';' : ''}
  ${this.selectedBlock.data.style.top ? 'top:' + this.selectedBlock.data.style.top + 'px;' : ''}
  ${this.selectedBlock.data.style.right ? 'right:' + this.selectedBlock.data.style.right + 'px;' : ''}
  ${this.selectedBlock.data.style.left ? 'left:' + this.selectedBlock.data.style.left + 'px;' : ''}
  ${this.selectedBlock.data.style.bottom ? 'bottom:' + this.selectedBlock.data.style.bottom + 'px;' : ''}
  ${this.selectedBlock.data.style.zIndex ? 'z-index:' + this.selectedBlock.data.style.zIndex + ';' : ''}
  `;

    /* flex */
    this.selectedBlock.data.css += `
  ${this.selectedBlock.data.style.flex ? 'flex:' + this.selectedBlock.data.style.flex + ';' : ''}
  ${this.selectedBlock.data.style.flexGrow ? 'flex-grow:' + this.selectedBlock.data.style.flexGrow + ';' : ''}
  ${this.selectedBlock.data.style.flexShrink ? 'flex-shrink:' + this.selectedBlock.data.style.flexShrink + ';' : ''}
  ${this.selectedBlock.data.style.flexBasis ? 'flex-basis:' + this.selectedBlock.data.style.flexBasis + ';' : ''}
  ${this.selectedBlock.data.style.flexDirection ? 'flex-direction:' + this.selectedBlock.data.style.flexDirection + ';' : ''}
  ${this.selectedBlock.data.style.flexWrap ? 'flex-wrap:' + this.selectedBlock.data.style.flexWrap + ';' : ''}
  ${this.selectedBlock.data.style.justifyContent ? 'justify-content:' + this.selectedBlock.data.style.justifyContent + ';' : ''}
  ${this.selectedBlock.data.style.alignItems ? 'align-items:' + this.selectedBlock.data.style.alignItems + ';' : ''}
  ${this.selectedBlock.data.style.alignContent ? 'align-content:' + this.selectedBlock.data.style.alignContent + ';' : ''}
  `;
    /* overflow */
    this.selectedBlock.data.css += `
  ${this.selectedBlock.data.style.overflow ? 'overflow:' + this.selectedBlock.data.style.overflow + ';' : ''}
  ${this.selectedBlock.data.style.overflowX ? 'overflow-x:' + this.selectedBlock.data.style.overflowX + ';' : ''}
  ${this.selectedBlock.data.style.overflowY ? 'overflow-y:' + this.selectedBlock.data.style.overflowY + ';' : ''}
  `;

    /* opacity */
    this.selectedBlock.data.css += `
  ${this.selectedBlock.data.style.opacity ? 'opacity:' + this.selectedBlock.data.style.opacity + ';' : ''}
  `;

    /* visibility */
    this.selectedBlock.data.css += `
  ${this.selectedBlock.data.style.visibility ? 'visibility:' + this.selectedBlock.data.style.visibility + ';' : ''}
  `;

    /* display */
    this.selectedBlock.data.css += `
  ${this.selectedBlock.data.style.display ? 'display:' + this.selectedBlock.data.style.display + ';' : ''}
  `;

    /* cursor */
    this.selectedBlock.data.css += `
  ${this.selectedBlock.data.style.cursor ? 'cursor:' + this.selectedBlock.data.style.cursor + ';' : ''}
  `;
  }
}

export class BlockRowData {
  cols: number = 2;
}

export class BlockProductCategoryData {
  title?: string;
  subTitle?: string;
}
