import { Injectable } from '@angular/core';
import { FileManagerService } from '@app/file-manager/file-manager.service';
import { AppConst } from '@shared/app-const';
import { IPageBuilderFilePicker } from 'ngx-page-builder';

@Injectable()
export class FilePickerService implements IPageBuilderFilePicker {
  baseUrlAddress: string = AppConst.fileServerUrl;
  constructor(private fileManager: FileManagerService) {}
  openFilePicker(type: 'image' | 'file'): Promise<string> {
    return new Promise((resolve, reject) => {
      this.fileManager
        .openFileManager({
          type: 'Image',
          maxSize: 1 * 1024, //1MB
          showPreview: true,
        })
        .then((file) => {
          if (file) {
            resolve('/' + file.id);
          } else {
            reject();
          }
        })
        .catch((er) => {
          reject(er);
        });
    });
  }
}
