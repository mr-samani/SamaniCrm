import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { IOptions } from '@app/file-manager/options.interface';
import { AppConst } from '@shared/app-const';
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
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}

  onSelectFile(fileId: string) {
    if (fileId) {
      this.images.push(
        new ProductImageDto({
          url: fileId,
        }),
      );
    }
  }
}
