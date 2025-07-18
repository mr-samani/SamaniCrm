import { Component, Input, ViewContainerRef, ComponentRef, ViewChild, inject, Renderer2 } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BLOCK_REGISTRY, BlockDefinition } from './block-registry';
import { FormBuilderService } from '../form-builder.service';
import { NgxResizableDirective } from 'ngx-drag-drop-kit';

@Component({
  selector: 'dynamic-renderer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="block-item"
      (click)="b.onSelect(block, $event)"
      [class.fb-selected]="b.selectedBlock == block"
      [class.hidden]="block.hidden">
      <div class="actions">
        <button (click)="b.deleteBlock(block, parent)">
          <i class="fa fa-trash"></i>
        </button>
      </div>
      <ng-container #container></ng-container>
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
      top: -28px;
      height: 28px;
      button {
        background: none;
        outline: none;
        border: none;
        cursor: pointer;
        color: inherit;
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
  block!: BlockDefinition;
  @ViewChild('container', { read: ViewContainerRef, static: true }) vcr!: ViewContainerRef;
  renderer = inject(Renderer2);
  @Input() parent?: BlockDefinition;
  @Input('block') set renderBlock(value: BlockDefinition) {
    this.block = value;
    if (!this.block.hidden) {
      this.vcr.clear();
      const def = BLOCK_REGISTRY.find((b) => b.type === this.block.type);
      if (def) {
        const { component } = def!;
        const cmpRef: ComponentRef<any> = this.vcr.createComponent(component!);
        cmpRef.instance.block = this.block;
      }
    } else {
      this.vcr.clear();
    }
  }
  constructor(public b: FormBuilderService) {}
}
