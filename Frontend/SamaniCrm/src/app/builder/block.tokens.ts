import { InjectionToken } from '@angular/core'; 
import { BlockDefinition } from './blocks/block-registry';

export const BLOCK_DATA = new InjectionToken<BlockDefinition>('BLOCK_DATA');
