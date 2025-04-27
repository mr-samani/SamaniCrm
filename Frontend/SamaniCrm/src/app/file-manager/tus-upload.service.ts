import { Injectable, Injector } from '@angular/core';
import { Upload, UploadOptions } from 'tus-js-client';
import { TokenService } from '../../shared/services/token.service';
import { AppConst } from '../../shared/app-const';
import { FileUsageEnum } from './image-cropper-dialog/image-cropper-dialog.component';
import { AppComponentBase } from '@app/app-component-base';

@Injectable()
export class TusUploadService extends AppComponentBase {
  uploading = false;
  progress = 0;
  uploadedUrl = '';
  chunckSize = 1 * 1024 * 1024; // 1MB
  constructor(
    injector: Injector,
    private _tokenService: TokenService,
  ) {
    super(injector);
  }

  reset() {
    this.progress = 0;
    this.uploadedUrl = '';
    this.uploading = false;
  }
  public uploadFile(
    file: File,
    token: string,
    usage: FileUsageEnum,
    additionalId?: string | number,
    parentId?: string | null,
  ): Promise<string> {
    return new Promise((resolve, reject) => {
      this.uploadedUrl = '';
      this.progress = 0;
      this.uploading = true;
      let metadata = {
        name: file.name,
        emptyMetaKey: '',
        size: file.size + '',
        chunkSize: this.chunckSize + '',
        usage: usage + '',
        additionalId: additionalId + '',
        parentId: parentId + '',
      };
      console.info('tus metadata', metadata);
      let upload = new Upload(file, {
        endpoint: AppConst.apiUrl + '/api/tus',
        retryDelays: [0, 1000, 2000, 5000],
        chunkSize: this.chunckSize,
        overridePatchMethod: true,
        onShouldRetry: (error: Error, retryAttempt: number, options: UploadOptions) => {
          // TODO
          let status = 0; // error.originalResponse ? error.originalResponse.getStatus() : 0;
          // Do not retry if the status is a 403.
          if ([417, 403, 401].indexOf(status) > -1) {
            return false;
          }
          return true;
        },
        metadata,
        headers: {
          Authorization: 'Bearer ' + this._tokenService.get().accessToken,
          fileToken: token,
        },
        onError: (error) => {
          this.alert.show({
            title: this.l('FileUpload'),
            text: this.l('Message.ErrorOccurred'),
          });
          console.error('Failed because: ' + error);
          this.uploading = false;
          // tslint:disable-next-line:forin
          for (let key in localStorage) {
            if (key.includes('tus')) {
              localStorage.removeItem(key);
            }
          }
          reject(error);
        },
        onSuccess: () => {
          // tus-js-client has bug: after error 417 dont call on error
          if (this.uploadedUrl) {
            this.uploading = false;
            // tslint:disable-next-line:forin
            for (let key in localStorage) {
              if (key.includes('tus')) {
                localStorage.removeItem(key);
              }
            }
            resolve(this.uploadedUrl);
          } else {
            this.alert.show({
              title: this.l('FileUpload'),
              text: this.l('Message.ErrorOccurred'),
            });
            reject();
          }
          this.uploading = false;
        },
        onAfterResponse: (req, res) => {
          // let value = res.getHeader('fileaddressurl');
          let value = res.getHeader('Fileid');
          if (value) {
            this.uploadedUrl = AppConst.apiUrl + '/' + value;
          }
        },
        onProgress: (bytesUploaded, bytesTotal) => {
          const percentage = ((bytesUploaded / bytesTotal) * 100).toFixed(0);
          this.progress = +percentage;
          // console.log(this.progress);
        },
      });
      upload.findPreviousUploads().then((previousUploads) => {
        this.uploadedUrl = '';
        this.uploading = true;
        if (previousUploads.length) {
          upload.resumeFromPreviousUpload(previousUploads[0]);
        }
        upload.start();
      });
    });
  }
}
