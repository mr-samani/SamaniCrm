import { Component, Input, OnInit } from '@angular/core';
import { BlockAttributeDetails, BlockDefinition, BlockTypeEnum } from '@app/builder/blocks/block-registry';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { IOptions } from '@app/file-manager/options.interface';
import { AppConst } from '@shared/app-const';
import { SimpleHtmlTags, CanChildHtmlTags } from '@app/builder/blocks/general-html-tags/GeneralTagNames';
import { FormBuilderService } from '@app/builder/services/form-builder.service';
import { IDataStructure } from '@app/builder/services/dynamic-data.service';

@Component({
  standalone: false,
  selector: 'block-attributes',
  templateUrl: './attributes.component.html',
  styleUrl: './attributes.component.scss',
})
export class BlockAttributesComponent implements OnInit {
  htmlTags = [...SimpleHtmlTags, ...CanChildHtmlTags];

  attrList: [string, BlockAttributeDetails][] = [];
  block!: BlockDefinition;
  @Input('block') set setBlock(val: BlockDefinition) {
    this.block = val;
    this.attrList = [];
    if (this.block.attributes) {
      this.attrList = Object.entries(this.block.attributes);
    }
    this.dynamicData = this.b.ds.getTreeDynamicDataList(this.block);
  }
  dynamicData: IDataStructure[] = [];
  selectedDynamicData: any = {};

  imagePickerOptions: IOptions = {
    type: 'Image',
    showPreview: false,
  };

  constructor(public b: FormBuilderService) {}

  ngOnInit(): void {}

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  onSelectFile(item: [string, BlockAttributeDetails], file: FileManagerDto) {
    if (file) {
      item[1].value = AppConst.fileServerUrl + '/' + file.id;
    }
  }
  onChangeDynamicKeyAttr(event: string[], item: [string, BlockAttributeDetails]) {
    item[1].value = `{{${event.join('.')}}}`;
    console.log(item[1].value);
  }

  onChangeDynamicKeyTxt(event: string[]) {
    if (this.block.data) {
      this.block.data.text = `{{${event.join('.')}}}`;
    }
    console.log(this.block.data?.text);
  }
}
