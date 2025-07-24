import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { FormBuilderService } from '@app/builder/form-builder.service';
import { BlockDefinition } from '../block-registry';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
@Component({
  selector: 'block-general-html-tags',
  standalone: true,
  imports: [CommonModule, DynamicRendererComponent, NgxDragDropKitModule],
  templateUrl: './general-html-tags.component.html',
  styles: `
    :host{
      display: block;
    }
    .block-wrapper {
      min-height: 20px;
    }
  `,
})
export class BlockGeneralHtmlTagsComponent implements OnInit {
  block?: BlockDefinition;
  index!: number;

  text = '';

  constructor(
    injector: Injector,
    public b: FormBuilderService,
  ) {}

  ngOnInit() {
    if (this.block) {
      if (!this.block.children) {
        this.block.children = [];
      }

      if (this.block.data?.text) {
        this.text = this.b.ds.resolveValue(this.block, this.block.data?.text, this.index);
      }
    }
  }
}
