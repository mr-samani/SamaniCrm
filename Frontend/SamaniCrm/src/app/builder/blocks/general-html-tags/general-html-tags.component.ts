import { ChangeDetectionStrategy, Component, ElementRef, Inject, Injector, OnInit, DOCUMENT } from '@angular/core';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { BlockDefinition } from '../block-registry';
import { FormBuilderService } from '@app/builder/services/form-builder.service';
@Component({
  selector: 'block-general-html-tags',
  standalone: true,
  imports: [NgxDragDropKitModule],
  template: `
    {{ text }}
  `,
  styles: `
    :host {
      display: block;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BlockGeneralHtmlTagsComponent implements OnInit {
  text = '';
  createdElement?: HTMLElement;
  block!: BlockDefinition;
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
    if (!this.block.children) {
      this.block.children = [];
    }
    this.update();
    app.event.on('block:update:' + this.block.id, () => this.update());
    app.event.on('block:updateText:' + this.block.id, () => this.update());
  }

  update() {
    if (!this.createdElement) {
      this.createdElement = this._doc.createElement(this.block.tagName || 'span');
    }
    if (this.createdElement && this.createdElement.tagName.toLowerCase() !== this.block.tagName?.toLowerCase()) {
      this.createdElement = undefined;
      this.update();
      return;
    }
    this.createdElement.innerHTML = this.text;
    this.createdElement.className = 'block-wrapper';
    this.el.nativeElement.innerHTML = '';
    this.el.nativeElement.append(this.createdElement);
    this.updateText();
  }

  updateText() {
    if (!this.createdElement) return;
    if (this.block.data?.text) {
      this.text = this.b.ds.resolveValue(this.block, this.block.data?.text, this.loopIndex);
      // console.log(this.block.data?.text, this.loopIndex, this.text);
    }
    this.createdElement.innerHTML = this.text;
  }
}
