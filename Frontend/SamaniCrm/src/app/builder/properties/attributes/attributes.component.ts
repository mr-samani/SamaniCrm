import { Component, Input } from '@angular/core';
import { BlockAttribute, BlockAttributeDetails } from '@app/builder/blocks/block-registry';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { IOptions } from '@app/file-manager/options.interface';
import { AppConst } from '@shared/app-const';

@Component({
  standalone: false,
  selector: 'block-attributes',
  templateUrl: './attributes.component.html',
  styleUrl: './attributes.component.scss',
})
export class BlockAttributesComponent {
  attrList: [string, BlockAttributeDetails][] = [];
  @Input() set attributes(val: BlockAttribute | undefined) {
    this.attrList = [];
    if (val) {
      this.attrList = Object.entries(val);
    }
  }

  imagePickerOptions: IOptions = {
    type: 'Image',
    showPreview: false,
  };

  onSelectFile(item: [string, BlockAttributeDetails], file: FileManagerDto) {
    if (file) {
      item[1].value = AppConst.fileServerUrl + '/' + file.id;
    }
  }
}
