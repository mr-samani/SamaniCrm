import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { BlockDefinition } from '../block-registry';
import { FormBuilderService } from '@app/builder/form-builder.service';
@Component({
  selector: 'block-general-html-tags',
  standalone: true,
  imports: [CommonModule, DynamicRendererComponent, NgxDragDropKitModule],
  templateUrl: './general-html-tags.component.html',
  styles: `
    :host {
      display: block;
    }
    .block-wrapper {
      min-height: 20px;
    }
  `,
})
export class BlockGeneralHtmlTagsComponent implements OnInit {
  text = '';
  block!: BlockDefinition;
  loopIndex!: number;
  constructor(
    injector: Injector,
    public b: FormBuilderService,
  ) {
    // super(injector);
  }

  ngOnInit() {
    if (this.block) {
      if (!this.block.children) {
        this.block.children = [];
      }

      if (this.block.data?.text) {
        this.text = this.b.ds.resolveValue(this.block, this.block.data?.text, this.loopIndex);
      }
    }
  }
}
