import {
  Component,
  Input,
  ViewContainerRef,
  ChangeDetectionStrategy,
  inject,
  ViewChild,
  AfterViewInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { BlockDefinition } from './block-registry';
import { CreateBlock } from '../helper/create-block';

@Component({
  selector: 'dynamic-renderer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <ng-container #container></ng-container>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DynamicRendererComponent implements AfterViewInit {
  block!: BlockDefinition;
  @ViewChild('container', { read: ViewContainerRef, static: false }) vcr!: ViewContainerRef;

  //private vcr = inject(ViewContainerRef);
  @Input('block') set AddBlock(value: BlockDefinition) {
    this.block = value;
  }

  ngAfterViewInit(): void {
    CreateBlock(this.vcr, this.block);
  }
}
