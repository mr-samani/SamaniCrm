import { Component, Injector, Input, OnInit } from '@angular/core';

import { BlockBase } from '../block-base';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'block-img',
  standalone: true,
  imports: [],
  template: `
    <img [src]="imageUrl" />
  `,
  styles: [
    `
      :host {
        display: contents;
      }
      img {
        max-width: 100%;
        height: auto;
        display: block;
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
    let fileId = this.b.ds.resolveValue(this.block, this.block.data?.text, this.loopIndex);
    if (fileId) {
      this.imageUrl = AppConst.fileServerUrl + '/' + fileId;
    }
  }
}
