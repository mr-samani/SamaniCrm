import { Component, Input, ViewContainerRef, ComponentRef, ViewChild, inject, Renderer2, Inject } from '@angular/core';
import { CommonModule, DOCUMENT } from '@angular/common';
import { BLOCK_REGISTRY, BlockData, BlockDefinition, BlockTypeEnum } from './block-registry';
import { FormBuilderService } from '../form-builder.service';
import { IResizableOutput } from 'ngx-drag-drop-kit';
import { BlockBase } from './block-base';
//  ngxResizable
//   (resizeEnd)="onResizeEnd($event)"
@Component({
  selector: 'dynamic-renderer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="block-item"
      (click)="onBlockClick(block, $event)"
      [class.fb-selected]="b.selectedBlock == block"
      [class.hidden]="block && block.hidden"
      [style]="block && block.data ? block.data.css : undefined">
      <div
        *ngIf="block"
        class="actions"
        [class.actions-top]="actionsPosition === 'top'"
        [class.actions-bottom]="actionsPosition === 'bottom'">
        <button (click)="b.deleteBlock(block, parent)">
          <i class="fa fa-trash"></i>
        </button>
        <span>
          {{ block.name ?? block.tagName ?? BlockTypeEnum[block.type] }}
        </span>
      </div>
      <ng-container #container></ng-container>
      <ng-content></ng-content>
    </div>
  `,
  styles: `
    :host {
      display: block;
      position: relative;
    }
    .actions {
      position: absolute;
      background: #3b97e3;
      color: #fff;
      padding: 4px 6px;
      display: none;
      height: 28px;
      z-index: 1000;
      button {
        background: none;
        outline: none;
        border: none;
        cursor: pointer;
        color: inherit;
      }
      &.actions-top {
        top: -28px;
      }
      &.actions-bottom {
        bottom: -28px;
      }
    }
    .block-item.fb-selected > .actions {
      display: block;
    }
    .block-item.hidden {
      display: none;
    }
  `,
})
export class DynamicRendererComponent {
  actionsPosition: 'top' | 'bottom' = 'top';
  block!: BlockDefinition;

  @ViewChild('container', { read: ViewContainerRef, static: true }) vcr!: ViewContainerRef;
  renderer = inject(Renderer2);
  @Input() parent?: BlockDefinition;
  @Input() index!: number;

  @Input('block') set renderBlock(value: BlockDefinition) {
    // wait for fill all inputs
   // setTimeout(() => {
      this.block = value;
      if (!this.block.hidden) {
        this.vcr.clear();
        const def = BLOCK_REGISTRY.find((b) => b.type === this.block.type);
        if (def) {
          const { component } = def!;
          const cmpRef: ComponentRef<BlockBase> = this.vcr.createComponent(component!);
          cmpRef.instance.block = this.block;
          cmpRef.instance.index = this.index;
          this.b.updateCss(this.block);
        }
      } else {
        this.vcr.clear();
      }
   // });
  }
  constructor(
    public b: FormBuilderService,
    @Inject(DOCUMENT) private doc: Document,
  ) {}

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  onBlockClick(block: BlockDefinition, event: Event) {
    this.updateActionsPosition(event.currentTarget as HTMLElement);
    this.b.onSelect(block, event);
  }

  updateActionsPosition(target: HTMLElement) {
    const el: HTMLElement | null = this.doc.querySelector('#builderCanvas');
    if (!el) return;
    const blockRect = target.getBoundingClientRect();
    const rect = el.getBoundingClientRect();

    this.actionsPosition = blockRect.top - 40 > rect.top ? 'top' : 'bottom';
  }
  onResizeEnd(event: IResizableOutput) {
    if (!this.b.selectedBlock) return;
    if (!this.b.selectedBlock.data) {
      this.b.selectedBlock.data = new BlockData();
    }

    if (!this.b.selectedBlock.data.style.position) {
      this.b.selectedBlock.data.style.position = 'relative';
    }

    this.b.selectedBlock.data.style.width = event.width;
    this.b.selectedBlock.data.style.height = event.height;
    // must be change in drag
    //this.b.selectedBlock.data.style.left = event.left;
    //this.b.selectedBlock.data.style.top = event.top;

    this.b.updateCss();
  }
}
