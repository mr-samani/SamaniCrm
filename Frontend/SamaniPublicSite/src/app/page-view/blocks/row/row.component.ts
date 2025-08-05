import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, inject, Injector, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { BlockBase } from '../block-base';
import { CreateBlock } from '@app/page-view/helper/create-block';
import { DynamicRendererComponent } from '../dynamic-renderer.component';

@Component({
  selector: 'block-row',
  standalone: true,
  imports: [CommonModule, DynamicRendererComponent],
  templateUrl: './row.component.html',
  styleUrl: './row.component.scss',
})
export class BlockRowComponent extends BlockBase implements OnInit {
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
