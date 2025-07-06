import { ComponentRef, ViewContainerRef } from '@angular/core';
import { BLOCK_REGISTRY, BlockDefinition } from '../blocks/block-registry';
import { throwError } from 'rxjs';

export function CreateBlock(vcr: ViewContainerRef, block: BlockDefinition) {
  if (!vcr) {
    throwError(() => Error('View Container not detect!'));
  } else {
    // vcr.clear();
    const comp = BLOCK_REGISTRY.get(block.type);
    if (comp) {
      const cmpRef: ComponentRef<any> = vcr.createComponent(comp);
      cmpRef.instance.block = block;
    }
  }
}
