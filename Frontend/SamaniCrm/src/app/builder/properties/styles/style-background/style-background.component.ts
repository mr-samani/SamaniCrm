import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AppConst } from '@shared/app-const';
import { IOptions } from '@app/file-manager/options.interface';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { BlockDefinition } from '@app/builder/blocks/block-registry';
declare const EyeDropper: any;
@Component({
  selector: 'style-background',
  templateUrl: './style-background.component.html',
  styleUrls: ['./style-background.component.scss'],
  standalone: false,
})
export class StyleBackgroundComponent implements OnInit {
  imageId?: string = '';
  backgroundGradient = '';
  backgroundType?: 'solidColor' | 'gradient' | 'image' | 'none';
  @Input() style!: Partial<CSSStyleDeclaration>;
  @Input() block!: BlockDefinition;
  @Output() styleChange = new EventEmitter<Partial<CSSStyleDeclaration>>();
  baseUrl = AppConst.fileServerUrl;
  fileSelectorOptions: IOptions = {
    type: 'Image',
    showPreview: false,
  };
  constructor() {}

  ngOnInit() {}

  clearBackground() {
    if (!this.style) return;
    this.style.backgroundColor = undefined;
    this.style.backgroundImage = undefined;
    this.style.backgroundPosition = undefined;
    this.style.backgroundRepeat = undefined;
    this.style.backgroundSize = undefined;
    this.backgroundType = 'none';
    this.imageId = '';
    this.backgroundGradient = '';
    this.emitChange();
  }

  private emitChange() {
    this.styleChange.emit(this.style);
  }

  update() {
    switch (this.backgroundType) {
      case 'none':
        this.clearBackground();
        this.style.backgroundColor = 'none';
        this.style.backgroundImage = 'none';
        break;
      case 'image':
        this.style.backgroundImage = `url('${this.baseUrl + '/' + this.imageId}')`;
        break;
      case 'gradient':
        this.style.backgroundImage = this.backgroundGradient;
        break;
    }
    this.emitChange();
  }
  onSelectFile(file: FileManagerDto) {
    if (file) {
      this.imageId = file.id;
    }
    this.update();
  }

}
