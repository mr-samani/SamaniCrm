import { CommonModule, DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, Injector, OnInit } from '@angular/core';
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
  _block?: BlockDefinition;
  set block(val: BlockDefinition) {
    setTimeout(() => {
      this._block = val;
      if (!this._block.children) {
        this._block.children = [];
      }

      if (this._block.data?.text) {
        this.text = this.b.ds.resolveValue(this._block, this._block.data?.text, this.loopIndex);
        // console.log(this._block.data?.text, this.loopIndex, this.text);
      }
    });
  }
  loopIndex!: number;
  constructor(
    injector: Injector,
    public b: FormBuilderService,
    @Inject(DOCUMENT) private _dom: Document,
    private el: ElementRef<HTMLElement>,
  ) {
    // super(injector);
  }

  ngOnInit() {}
}
