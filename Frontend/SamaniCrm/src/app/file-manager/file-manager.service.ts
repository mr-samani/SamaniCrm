import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {
  ImageCropperDialogData,
  ImageCropperDialogComponent,
} from './image-cropper-dialog/image-cropper-dialog.component';
import { IOptions } from './options.interface';
import { FileManagerComponent } from './file-manager/file-manager.component';
import { FileManagerDto } from './models/file-manager-dto';

@Injectable()
export class FileManagerService {
  constructor(private matDialog: MatDialog) {}

  /**
   * select file from file manager
   * @param data options
   * @returns fileId
   */
  selectFile(data: ImageCropperDialogData): Promise<string> {
    return new Promise((resolve, reject) => {
      this.matDialog
        .open(ImageCropperDialogComponent, {
          data,
        })
        .afterClosed()
        .subscribe((result) => {
          resolve(result);
        });
    });
  }

  /**
   * # انتخاب فایل از فایل منیجر
   *
   * @returns Guid file
   */
  public openFileManager(options?: IOptions): Promise<FileManagerDto> {
    return new Promise<FileManagerDto>((resolve, reject) => {
      this.matDialog
        .open(FileManagerComponent, {
          data: options,
          width: '80vw',
          height: '90%',
          closeOnNavigation: true,
          disableClose: false,
        })
        .afterClosed()
        .subscribe((result) => {
          if (result) {
            resolve(result);
          } else {
            reject();
          }
        });
    });
  }
}
