import {
  Component,
  Input,
  ViewContainerRef,
  ComponentRef,
  ViewChild,
  inject,
  Renderer2,
  Inject,
  ElementRef,
} from '@angular/core';
import { CommonModule, DOCUMENT } from '@angular/common';
import { BLOCK_REGISTRY, BlockData, BlockDefinition, BlockTypeEnum } from './block-registry';
import { FormBuilderService } from '../form-builder.service';
import { IResizableOutput, NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { BlockBase } from './block-base';
//  ngxResizable
//   (resizeEnd)="onResizeEnd($event)"
@Component({
  selector: 'dynamic-renderer',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule],
  host: {
    '[class.fb-selected]': 'b.selectedBlock?.id == block?.id',
    '[class.hidden]': 'block && block.hidden',
    '[style]': 'block && block.data ? block.data.css : undefined',
  },
  templateUrl: './dynamic-renderer.component.html',
  styleUrls: ['./dynamic-renderer.component.scss'],
})
export class DynamicRendererComponent {
  actionsPosition: 'top' | 'bottom' = 'top';
  block!: BlockDefinition;

  @ViewChild('container', { read: ViewContainerRef, static: true }) vcr!: ViewContainerRef;
  renderer = inject(Renderer2);
  @Input() parent?: BlockDefinition;
  @Input() loopIndex!: number;

  @Input('block') set renderBlock(value: BlockDefinition) {
    // wait for fill all inputs
    setTimeout(() => {
      this.block = value;
      this.createBlock();
    });
  }
  constructor(
    public b: FormBuilderService,
    @Inject(DOCUMENT) private doc: Document,
    el: ElementRef<HTMLElement>,
  ) {
    el.nativeElement.addEventListener('click', (ev: Event) => {
      this.onBlockClick(this.block, ev);
    });
  }

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  createBlock() {
    this.vcr.clear();
    if (!this.block.hidden) {
      const def = BLOCK_REGISTRY.find((b) => b.type === this.block.type);
      if (def) {
        const { component } = def!;
        const cmpRef: ComponentRef<BlockBase> = this.vcr.createComponent(component!);
        cmpRef.instance.block = this.block;
        cmpRef.instance.loopIndex = this.loopIndex;
        this.b.updateCss(this.block);
      }
    }
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
