import { Component, Injector, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BlockBase } from '../block-base';

@Component({
  selector: 'block-img',
  standalone: true,
  imports: [CommonModule],
  template: `
    <img [src]="imageUrl" />
  `,
  styles: [
    `
      :host {
        display: inline-block;
      }
    `,
  ],
})
export class BlockImgComponent extends BlockBase implements OnInit {
  imageUrl = 'images/default-image.png';

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    if (!this.block.attributes) {
      this.block.attributes = {};
    }
    this.block.attributes.url ??= { type: 'image', value: 'images/default-image.png' };
    this.imageUrl = this.b.ds.resolveValue(this.block, this.block.attributes.url.value, this.loopIndex);
  }
}
