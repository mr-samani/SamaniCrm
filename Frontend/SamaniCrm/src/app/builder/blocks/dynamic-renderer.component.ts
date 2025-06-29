import { Component, Input, ViewContainerRef, ComponentRef, inject, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BLOCK_REGISTRY, BlockDefinition } from './block-registry';

@Component({
  selector: 'app-dynamic-renderer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <ng-container #container></ng-container>
  `,
})
export class DynamicRendererComponent implements OnChanges {
  @Input() blocks: BlockDefinition[] = [];
  private vcr = inject(ViewContainerRef);

  async ngOnChanges(_: SimpleChanges) {
    this.vcr.clear();
    for (const block of this.blocks) {
      const def = BLOCK_REGISTRY.find((b) => b.type === block.type);
      if (def) {
        const { component } = def!;
        const cmpRef: ComponentRef<any> = this.vcr.createComponent(component!);
        cmpRef.instance.block = block;
      }
    }
  }
}
