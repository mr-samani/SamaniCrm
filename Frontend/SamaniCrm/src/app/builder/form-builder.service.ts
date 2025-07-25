import { Injectable } from '@angular/core';
import { BlockTypeEnum, BLOCK_REGISTRY, BlockDefinition, FormTools, BlockData } from './blocks/block-registry';
import { IDropEvent, moveItemInArray } from 'ngx-drag-drop-kit';
import { ViewModeEnum } from './models/view-mode.enum';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { createTreeFormTools } from './helpers/tools';
import { PageBuilderServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';
import { CanChildHtmlTags, SimpleHtmlTags } from './blocks/general-html-tags/GeneralTagNames';
import { cloneDeep } from 'lodash-es';
import { generateSequentialGuid } from '@shared/helper/guid';
import { DynamicDataService } from './dynamic-data.service';
import { BlockGeneralHtmlTagsComponent } from './blocks/general-html-tags/general-html-tags.component';

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
    // important create new id
    s.id = generateSequentialGuid();
    s.data ??= new BlockData();
    parentChildren.splice(index, 0, new BlockDefinition(s));

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

  updateCss(block?: BlockDefinition) {
    if (!block) {
      block = this.selectedBlock;
    }
    if (!block) return;

    if (!block.data) {
      block.data = new BlockData();
    }
    block.data.css = `
  ${block.data.style.border ? 'border:' + block.data.style.border + ';' : ''}
  ${block.data.style.padding ? 'padding:' + block.data.style.padding + ';' : ''}
  ${block.data.style.margin ? 'margin:' + block.data.style.margin + ';' : ''}
  ${block.data.style.borderRadius ? 'border-radius:' + block.data.style.borderRadius + ';' : ''}
  ${block.data.style.boxShadow ? 'box-shadow:' + block.data.style.boxShadow + ';' : ''}
  ${block.data.style.backgroundColor ? 'background-color:' + block.data.style.backgroundColor + ';' : ''}
  ${block.data.style.backgroundImage ? 'background-image:' + block.data.style.backgroundImage + ';' : ''}
  ${block.data.style.backgroundSize ? 'background-size:' + block.data.style.backgroundSize + ';' : ''}
  ${block.data.style.backgroundRepeat ? 'background-repeat:' + block.data.style.backgroundRepeat + ';' : ''}`;

    /* dimensions */
    block.data.css += `
  ${block.data.style.width ? 'width:' + block.data.style.width + 'px;' : ''}
  ${block.data.style.height ? 'height:' + block.data.style.height + 'px;' : ''}
  ${block.data.style.minWidth ? 'min-width:' + block.data.style.minWidth + 'px;' : ''}
  ${block.data.style.minHeight ? 'min-height:' + block.data.style.minHeight + 'px;' : ''}
  ${block.data.style.maxWidth ? 'max-width:' + block.data.style.maxWidth + 'px;' : ''}
  ${block.data.style.maxHeight ? 'max-height:' + block.data.style.maxHeight + 'px;' : ''}`;

    /* position */
    block.data.css += `
  ${block.data.style.position ? 'position:' + block.data.style.position + ';' : ''}
  ${block.data.style.top ? 'top:' + block.data.style.top + 'px;' : ''}
  ${block.data.style.right ? 'right:' + block.data.style.right + 'px;' : ''}
  ${block.data.style.left ? 'left:' + block.data.style.left + 'px;' : ''}
  ${block.data.style.bottom ? 'bottom:' + block.data.style.bottom + 'px;' : ''}
  ${block.data.style.zIndex ? 'z-index:' + block.data.style.zIndex + ';' : ''}
  `;

    /* flex */
    block.data.css += `
  ${block.data.style.flex ? 'flex:' + block.data.style.flex + ';' : ''}
  ${block.data.style.flexGrow ? 'flex-grow:' + block.data.style.flexGrow + ';' : ''}
  ${block.data.style.flexShrink ? 'flex-shrink:' + block.data.style.flexShrink + ';' : ''}
  ${block.data.style.flexBasis ? 'flex-basis:' + block.data.style.flexBasis + ';' : ''}
  ${block.data.style.flexDirection ? 'flex-direction:' + block.data.style.flexDirection + ';' : ''}
  ${block.data.style.flexWrap ? 'flex-wrap:' + block.data.style.flexWrap + ';' : ''}
  ${block.data.style.justifyContent ? 'justify-content:' + block.data.style.justifyContent + ';' : ''}
  ${block.data.style.alignItems ? 'align-items:' + block.data.style.alignItems + ';' : ''}
  ${block.data.style.alignContent ? 'align-content:' + block.data.style.alignContent + ';' : ''}
  ${block.data.style.gap ? 'gap:' + block.data.style.gap + ';' : ''}
  `;
    /* overflow */
    block.data.css += `
  ${block.data.style.overflow ? 'overflow:' + block.data.style.overflow + ';' : ''}
  ${block.data.style.overflowX ? 'overflow-x:' + block.data.style.overflowX + ';' : ''}
  ${block.data.style.overflowY ? 'overflow-y:' + block.data.style.overflowY + ';' : ''}
  `;

    /* opacity */
    block.data.css += `
  ${block.data.style.opacity ? 'opacity:' + block.data.style.opacity + ';' : ''}
  `;

    /* visibility */
    block.data.css += `
  ${block.data.style.visibility ? 'visibility:' + block.data.style.visibility + ';' : ''}
  `;

    /* display */
    block.data.css += `
  ${block.data.style.display ? 'display:' + block.data.style.display + ';' : ''}
  `;

    /* cursor */
    block.data.css += `
  ${block.data.style.cursor ? 'cursor:' + block.data.style.cursor + ';' : ''}
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
