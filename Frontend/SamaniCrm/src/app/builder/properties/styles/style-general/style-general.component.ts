import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BlockDefinition } from '@app/builder/blocks/block-registry';
import { AppConst } from '@shared/app-const';
import { TextAlignItems } from './TextAlignItems';
import { FlexDirectionItems } from './FlexDirectionItems';
import { InputGroupItem } from '../input-group-icon/InputGroupItem';

@Component({
  selector: 'style-general',
  templateUrl: './style-general.component.html',
  styleUrls: ['./style-general.component.scss'],
  standalone: false,
})
export class StyleGeneralComponent implements OnInit {
  @Input() style!: Partial<CSSStyleDeclaration>;
  @Input() block!: BlockDefinition;
  @Output() styleChange = new EventEmitter<Partial<CSSStyleDeclaration>>();
  isRtl = AppConst.isRtl;
  textAlignItems = TextAlignItems;
  flexDirectionItems = FlexDirectionItems;
  justifyContentItems = JustifyContentItems;
  constructor() {}

  ngOnInit() {}

  private emitChange() {
    this.styleChange.emit(this.style);
  }

  update() {
    this.emitChange();
  }

  clearStyle(styleKey: keyof Partial<CSSStyleDeclaration>) {
    if (!this.style) return;
    delete this.style[styleKey];
  }
}

export const JustifyContentItems: InputGroupItem[] = [
  { icon: '', value: 'flex-start' },
  { icon: '', value: 'flex-end' },
  { icon: '', value: 'center' },
  { icon: '', value: 'space-between' },
  { icon: '', value: 'space-around' },
  { icon: '', value: 'space-evenly' },
];
