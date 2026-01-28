import { ElementRef, inject,} from '@angular/core';
import { FormBuilderService } from '../services/form-builder.service';
import { BlockDefinition } from './block-registry';

export class BlockBase {
  block!: BlockDefinition;
  loopIndex!: number;
  b = inject(FormBuilderService);
  el = inject(ElementRef<HTMLElement>);
  constructor() {}
}
