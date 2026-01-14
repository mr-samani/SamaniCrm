import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { IOptions } from '@app/file-manager/options.interface';
import { AppConst } from '@shared/app-const';
import { ProductFileDto } from '@shared/service-proxies/model/product-file-dto';

@Component({
  selector: 'product-files',
  templateUrl: './product-files.component.html',
  styleUrls: ['./product-files.component.scss'],
  standalone: false,
})
export class ProductFilesComponent extends AppComponentBase implements OnInit {
  @Input() files: ProductFileDto[] = [];
  @Output() filesChange = new EventEmitter<ProductFileDto[]>();

  fileServerUrl = AppConst.fileServerUrl;
  fileSelectorOptions: IOptions = {
    type: 'All',
    showPreview: false,
  };
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}

  onSelectFile(file: FileManagerDto) {
    if (file) {
      if (this.files.findIndex((x) => x.fileId == file.id) > -1) {
        this.notify.warning(this.l('SelectedFileIsDuplicate'));
        return;
      }

      this.files.push(
        new ProductFileDto({
          fileId: file.id,
          description: file.name,
        }),
      );
    }
  }

  removeFile(event: Event, item: ProductFileDto) {
    event.stopPropagation();
    event.preventDefault();
    const index = this.files.findIndex((x) => x.id == item.id);
    if (index > -1) {
      this.files.splice(index, 1);
    }
  }
}
