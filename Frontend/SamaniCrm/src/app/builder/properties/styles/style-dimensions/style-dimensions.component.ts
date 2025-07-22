import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BlockStyle } from '../models/_style';
import { AppConst } from '@shared/app-const';
import { IOptions } from '@app/file-manager/options.interface';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { BlockDefinition } from '@app/builder/blocks/block-registry';
declare const EyeDropper: any;
@Component({
  selector: 'style-dimensions',
  templateUrl: './style-dimensions.component.html',
  styleUrls: ['./style-dimensions.component.scss'],
  standalone: false,
})
export class StyleDimensionsComponent implements OnInit {
  @Input() style!: BlockStyle;
  @Input() block!: BlockDefinition;
  @Output() styleChange = new EventEmitter<BlockStyle>();

  constructor() {}

  ngOnInit() {}



  private emitChange() {
    this.styleChange.emit(this.style);
  }

  update() {
  
    
    this.emitChange();
  }
  
}
