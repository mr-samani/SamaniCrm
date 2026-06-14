import { Component, EventEmitter,  Input, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { IOptions } from '@app/file-manager/options.interface';
import { AppConst } from '@shared/app-const';
import { ProductPriceDto } from '@shared/service-proxies';
import { ProductImageDto } from '@shared/service-proxies/model/product-image-dto';

@Component({
  selector: 'product-images',
  templateUrl: './product-images.component.html',
  styleUrls: ['./product-images.component.scss'],
  standalone: false,
})
export class ProductImagesComponent extends AppComponentBase implements OnInit {
  @Input() images: ProductImageDto[] = [];
  @Output() imagesChange = new EventEmitter<ProductImageDto[]>();

  fileServerUrl = AppConst.fileServerUrl;
  fileSelectorOptions: IOptions = {
    type: 'Image',
    showPreview: false,
  };
  constructor() {
    super();
  }

  ngOnInit() {}

  onSelectFile(file: FileManagerDto) {
    if (file) {
      if (this.images.findIndex((x) => x.fileId == file.id) > -1) {
        this.notify.warning(this.l('SelectedFileIsDuplicate'));
        return;
      }

      const item = new ProductImageDto({
        fileId: file.id,
      });
      if (this.images.findIndex((x) => x.isMain) == -1) {
        item.isMain = true;
      }
      this.images.push(item);
    }
  }

  setAsDefault(item: ProductImageDto) {
    this.images.map((m) => (m.isMain = false));
    item.isMain = true;
  }

  removeFile(event: Event, item: ProductPriceDto) {
    event.stopPropagation();
    event.preventDefault();
    const index = this.images.findIndex((x) => x.id == item.id);
    if (index > -1) {
      this.images.splice(index, 1);
    }
  }
}
