import { AfterViewInit, Component, Inject,  OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ImageCroppedEvent, OutputFormat, base64ToFile } from 'ngx-image-cropper';
import { TusUploadService } from '../tus-upload.service';
import { AppComponentBase } from '@app/app-component-base';

export enum FileUsageEnum {
  FILE_MANAGER = 0,
  USER_AVATAR = 1,
  PRODUCT_CATEGORY = 2,
  HTML_EDITOR = 3,
}
export interface ImageCropperDialogData {
  maxFileSizeKB?: number;
  maintainAspectRatio?: boolean;
  tokenId?: string;
  previousFileAddress?: string;
  /**
   * - true => return uploaded url
   * - false => return base64string
   */
  uploadWithTus?: boolean;
  // disable resize = 0
  maximumWidth?: number;
  maximumHeight?: number;

  usage: FileUsageEnum;

  additionalId?: string | number;
  /*
   * file manager parent id
   */
  parentId?: string | null;
}
@Component({
  selector: 'app-image-cropper-dialog',
  templateUrl: './image-cropper-dialog.component.html',
  styleUrls: ['./image-cropper-dialog.component.scss'],
  standalone: false,
})
export class ImageCropperDialogComponent extends AppComponentBase implements OnInit, AfterViewInit {
  public maxFileSizeKB = 5 * 1024; // 5MB
  public maintainAspectRatio = true;
  imageChangedEvent: any = '';
  imageCroppedEvent?: ImageCroppedEvent;
  allowedFileTypes = ['image/png', 'image/jpg', 'image/gif', 'image/jpeg'];
  acceptedInputFile = '.png,.jpg,.jpeg,.gif';

  previousFileAddress = '';

  /**
   * در صورتی که بخواهیم فقط فایل تصویر را بگیریم
   * این گزینه را
   * false
   * پاس میدیم
   */
  uploadWithTus = true;
  maximumWidth = 0;
  maximumHeight = 0;
  format: OutputFormat = 'jpeg';
  fileType = 'image/jpeg';
  fileName = '';
  usage: FileUsageEnum;
  additionalId?: string | number;
  parentId?: string | null;

  constructor(
    private matDialogRef: MatDialogRef<ImageCropperDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: ImageCropperDialogData,
    public tusUpoadService: TusUploadService,
  ) {
    super();
    this.uploadWithTus = _data.uploadWithTus === false ? false : true;
    this.usage = _data.usage;
    this.additionalId = _data.additionalId;
    this.parentId = _data.parentId;
    if (_data.maximumWidth && _data.maximumHeight) {
      this.maximumWidth = _data.maximumWidth;
      this.maximumHeight = _data.maximumHeight;
    }

    if (this._data.maxFileSizeKB) {
      this.maxFileSizeKB = this._data.maxFileSizeKB;
    }
    this.maintainAspectRatio = this._data.maintainAspectRatio === true;

    this.previousFileAddress = this._data.previousFileAddress ?? '';
    // reset tus service data
    this.tusUpoadService.reset();
  }

  ngOnInit(): void {}
  ngAfterViewInit(): void {
    const fileSelector = document.getElementById('cropperFileSelector');
    if (fileSelector) {
      fileSelector.click();
    }
  }

  fileChangeEvent(event: any): void {
    if (this.allowedFileTypes.indexOf(event.target.files[0].type) === -1) {
      this.notify.warning(this.l('FileTypeIsNotAllowed') + this.l('SelectOnlyFile{0}', 'Jpg , png , gif'));
      return;
    }
    if (event.target.files[0].size > this.maxFileSizeKB * 1024) {
      this.notify.warning(this.l('ProfilePicture_Warn_SizeLimit', this.maxFileSizeKB));
      return;
    }
    this.fileName = event.target.files[0].name + '';
    this.format = this.fileName.split('.').pop()?.toLowerCase() ?? ('' as any);
    this.fileType = event.target.files[0].type;
    this.imageChangedEvent = event;
  }

  imageCroppedFile(event: ImageCroppedEvent) {
    this.imageCroppedEvent = event;
  }

  uploadImage() {
    if (this.imageCroppedEvent) {
      this.compressImage(this.imageCroppedEvent.blob!, this.fileName, { quality: 0.6, type: this.fileType }).then(
        (compressedResult) => {
          if (this.uploadWithTus) {
            this.tusUpoadService
              .uploadFile(compressedResult, '', this.usage, this.additionalId, this.parentId)
              .then((result) => {
                this.notify.success(this.l('Message.UploadedSuccessFully'));
                this.matDialogRef.close(result);
              })
              .catch((error) => {
                this.notify.error(this.l('Message.AnErrorOccurred'));
              });
          } else {
            //convert file to base 64
            this.toBase64(compressedResult).then((base64) => {
              this.matDialogRef.close(base64);
            });
          }
        },
      );
    } else {
      this.matDialogRef.close();
    }
  }

  async compressImage(file: Blob, filename: string, { quality = 1, type = file.type }): Promise<File> {
    // Get as image data
    const imageBitmap = await createImageBitmap(file, { premultiplyAlpha: 'premultiply' });

    // Draw to canvas
    const canvas = document.createElement('canvas');
    canvas.width = imageBitmap.width;
    canvas.height = imageBitmap.height;
    const ctx = canvas.getContext('2d')!;
    ctx.globalAlpha = 1;
    ctx.globalCompositeOperation = 'destination-over';
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(imageBitmap, 0, 0);

    // Turn into Blob
    return new Promise((resolve) => {
      return canvas.toBlob(
        (result: any) => {
          return resolve(new File([result], filename, { type: type }));
        },
        type,
        quality,
      );
    });
  }

  toBase64(file: File) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = reject;
    });
  }
}
