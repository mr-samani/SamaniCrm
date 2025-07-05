import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { FormBuilderService } from '@app/builder/form-builder.service';
import { BlockDefinition } from '../block-registry';

@Component({
  selector: 'block-div',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule, DynamicRendererComponent],
  templateUrl: './div.component.html',
  styles: `
    :host {
      display: block;
      padding: 5px;
    }
    .block-wrapper {
      min-height: 80px;
    }
  `,
})
export class BlockDivComponent implements OnInit {
  block?: BlockDefinition;
  constructor(
    injector: Injector,
    public b: FormBuilderService,
  ) {}

  ngOnInit() {
    if (this.block && !this.block.children) {
      this.block.children = [];
    }
  }
}
