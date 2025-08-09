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
  HostListener,
} from '@angular/core';
import { CommonModule, DOCUMENT } from '@angular/common';
import { BLOCK_REGISTRY, BlockDefinition, BlockTypeEnum } from './block-registry';
import { FormBuilderService } from '../services/form-builder.service';
import { IDropEvent, NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { BlockBase } from './block-base';
import { OverlayCanvasService } from '../services/overlay-canvas.service';
//  ngxResizable
//   (resizeEnd)="onResizeEnd($event)"
@Component({
  selector: 'dynamic-renderer',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule],
  host: {
    '[class.hidden]': 'block && block.hidden',
  },
  templateUrl: './dynamic-renderer.component.html',
  styleUrls: ['./dynamic-renderer.component.scss'],
})
export class DynamicRendererComponent {
  actionsPosition: 'top' | 'bottom' = 'top';
  block?: BlockDefinition;

  @ViewChild('container', { read: ViewContainerRef, static: true }) vcr!: ViewContainerRef;
  renderer = inject(Renderer2);
  @Input() parent?: BlockDefinition;
  @Input() loopIndex!: number;
  @Input() canDrag = true;
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
    private el: ElementRef<HTMLElement>,
    private overlayCanvasService: OverlayCanvasService,
  ) {
    el.nativeElement.addEventListener('click', (ev: Event) => {
      this.onBlockClick(this.block!, ev);
    });
  }

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  createBlock() {
    this.vcr.clear();
    if (!this.block?.hidden) {
      const def = BLOCK_REGISTRY.find((b) => b.type === this.block?.type);
      if (def) {
        const { component } = def!;
        const cmpRef: ComponentRef<BlockBase> = this.vcr.createComponent(component!);
        cmpRef.instance.loopIndex = this.loopIndex;
        cmpRef.instance.block = this.block!;
        this.b.updateCss(this.block, false);
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
    const blockRect = this.el.nativeElement.getBoundingClientRect();
    const rect = el.getBoundingClientRect();

    this.actionsPosition = blockRect.top - 40 > rect.top ? 'top' : 'bottom';
  }

  drop($event: IDropEvent<BlockDefinition[]>) {
    this.b.drop($event, this.block);
  }

  @HostListener('mouseover')
  onMouseOver() {
    let target = this.doc.querySelector('#block_' + this.block?.id);
    if (!target) return;
    this.overlayCanvasService.highlightElement(target, { label: true, side: ['all'], type: ['margin', 'padding'] });
  }

  @HostListener('mouseout')
  onMouseOut() {
    this.overlayCanvasService.clear();
  }
}
