import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';

import { BlockBase } from '../block-base';
import { DynamicRendererComponent } from '../dynamic-renderer.component';

@Component({
  selector: 'block-div',
  standalone: true,
  imports: [CommonModule, DynamicRendererComponent],
  templateUrl: './div.component.html',
  styles: `
    :host {
      display: block;
      padding: 5px;
    } 
  `,
})
export class BlockDivComponent extends BlockBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    if (this.block) {
      if (!this.block.children) {
        this.block.children = [];
      }
    }
  }
}
