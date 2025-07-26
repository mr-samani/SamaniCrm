import { CommonModule, DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, Injector, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { BlockDefinition } from '../block-registry';
import { FormBuilderService } from '@app/builder/form-builder.service';
@Component({
  selector: 'block-general-html-tags',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule],
  template: `
    <div class="block-wrapper">{{ text }}</div>
  `,
  styles: `
    :host {
      display: block;
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
    @Inject(DOCUMENT) private _dom: Document,
    private el: ElementRef<HTMLElement>,
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
        console.log(this.block.data?.text, this.loopIndex, this.text);
      }
    }
  }
}
