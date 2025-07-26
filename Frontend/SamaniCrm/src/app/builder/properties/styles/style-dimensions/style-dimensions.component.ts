import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BlockDefinition } from '@app/builder/blocks/block-registry';
@Component({
  selector: 'style-dimensions',
  templateUrl: './style-dimensions.component.html',
  styleUrls: ['./style-dimensions.component.scss'],
  standalone: false,
})
export class StyleDimensionsComponent implements OnInit {
  @Input() style!: Partial<CSSStyleDeclaration>;
  @Input() block!: BlockDefinition;
  @Output() styleChange = new EventEmitter<Partial<CSSStyleDeclaration>>();

  constructor() {}

  ngOnInit() {}

  private emitChange() {
    this.styleChange.emit(this.style);
  }

  update() {
    this.emitChange();
  }
}
