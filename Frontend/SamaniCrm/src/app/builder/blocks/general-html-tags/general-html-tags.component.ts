import { CommonModule, DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, Injector, OnInit } from '@angular/core';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { BlockDefinition } from '../block-registry';
import { FormBuilderService } from '@app/builder/services/form-builder.service';
@Component({
  selector: 'block-general-html-tags',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule],
  template: `
    {{ text }}
  `,
  styles: `
    :host {
      display: block;
    }
  `,
})
export class BlockGeneralHtmlTagsComponent implements OnInit {
  text = '';
  createdElement?: HTMLElement;
  _block?: BlockDefinition;
  set block(val: BlockDefinition) {
    setTimeout(() => {
      this._block = val;
      if (!this._block.children) {
        this._block.children = [];
      }

      this.update();
    });
  }
  loopIndex!: number;
  constructor(
    injector: Injector,
    public b: FormBuilderService,
    @Inject(DOCUMENT) private _doc: Document,
    private el: ElementRef<HTMLElement>,
  ) {
    // super(injector);
  }

  ngOnInit() {
    app.event.on('event_' + this._block?.id, () => this.update());
  }

  update() {
    if (!this._block) return;
    if (this._block.data?.text) {
      this.text = this.b.ds.resolveValue(this._block, this._block.data?.text, this.loopIndex);
      // console.log(this._block.data?.text, this.loopIndex, this.text);
    }

    if (!this.createdElement) {
      this.createdElement = this._doc.createElement(this._block.tagName || 'span');
    }
    if (this.createdElement && this.createdElement.tagName.toLowerCase() !== this._block.tagName?.toLowerCase()) {
      this.createdElement = undefined;
      this.update();
      return;
    }
    this.createdElement.innerHTML = this.text;
    this.createdElement.className = 'block-wrapper';
    this.el.nativeElement.innerHTML = '';
    this.el.nativeElement.append(this.createdElement);
  }
}
