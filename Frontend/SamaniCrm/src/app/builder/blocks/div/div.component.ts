import { CommonModule } from '@angular/common';
import { Component, Injector, Input, OnInit } from '@angular/core';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { BlockBase } from '../block-base';
import { FormBuilderService } from '@app/builder/form-builder.service';
import { BlockDefinition, IBlockDefinition } from '../block-registry';

@Component({
  selector: 'block-div',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule, DynamicRendererComponent],
  templateUrl: './div.component.html',
  styles: `
    .div {
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
    // Initialization logic here
  }
}
