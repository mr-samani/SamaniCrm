import { Injectable, Injector } from '@angular/core';
import { DetailedError, Upload, UploadOptions } from 'tus-js-client';
import { AppConst } from '../../shared/app-const';
import { FileUsageEnum } from './image-cropper-dialog/image-cropper-dialog.component';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { LanguageService } from '@shared/services/language.service';
import { AuthService } from '@shared/services/auth.service';
import * as tus from 'tus-js-client';
@Injectable()
export class TusUploadService {
  uploading = false;
  progress = 0;
  uploadedUrl = '';
  chunckSize = 1 * 1024 * 1024; // 1MB

  upload?: Upload;
  constructor(
    private alert: NgxAlertModalService,
    private language: LanguageService,
    private authService: AuthService,
  ) {}

  reset() {
    this.progress = 0;
    this.uploadedUrl = '';
    this.uploading = false;
  }
  l(key: string, param?: Object) {
    // console.log(this.language.translate.instant(key, param));
    return this.language.translate.instant(key, param);
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

      const filetype = file.name.split('.').pop() ?? '';
      const metadata = {
        filename: file.name,
        filetype: filetype,
        emptyMetaKey: '',
        size: file.size + '',
        chunkSize: this.chunckSize + '',
        usage: usage ? usage + '' : '',
        additionalId: additionalId ? additionalId + '' : '',
        parentId: parentId + '',
      };
      console.info('tus metadata', metadata);

      // تابع برای ساخت upload instance
      const createUpload = (): Upload => {
        return new Upload(file, {
          endpoint: AppConst.apiUrl + '/api/tus',
          retryDelays: [0, 1000, 2000, 5000],
          chunkSize: this.chunckSize,
          overridePatchMethod: true,
          metadata,
          headers: {
            fileToken: token,
          },
          onBeforeRequest: (req) => {
            const xhr = req.getUnderlyingObject();
            if (xhr instanceof XMLHttpRequest) {
              xhr.withCredentials = true;
            }
          },
          onShouldRetry: (err: DetailedError, retryAttempt: number, options: UploadOptions) => {
            const status = err.originalResponse ? err.originalResponse.getStatus() : 0;

            if (status === 401) {
              //return this.handle401AndRetry(options);
              return false;
            }
            if ([417, 403, 400].indexOf(status) > -1) {
              return false;
            }

            return true;
          },
          onError: (error: DetailedError | any) => {
            const status = error.originalResponse ? error.originalResponse.getStatus() : 0;
            let responseBody = error.originalResponse ? error.originalResponse.getBody() : '';
            try {
              responseBody = JSON.parse(responseBody);
            } catch (error) {
              responseBody = {};
            }

            this.alert.show({
              title: this.l('FileUpload') + ' ' + responseBody.error + ' (' + status + ')',
              text: responseBody.message,
            });
            console.error('Failed because: ' + error);
            this.uploading = false;
            this.clearTusLocalStorage();
            reject(error);
          },
          onSuccess: () => {
            if (this.uploadedUrl) {
              this.uploading = false;
              this.clearTusLocalStorage();
              resolve(this.uploadedUrl);
            } else {
              this.alert.show({
                title: this.l('FileUpload'),
                text: this.l('Message.ErrorOccurred'),
              });
              this.uploading = false;
              reject(new Error('Upload completed but no URL returned'));
            }
          },
          onAfterResponse: (req, res) => {
            const value = res.getHeader('Fileid');
            if (value) {
              this.uploadedUrl = AppConst.apiUrl + '/' + value;
            }
          },
          onProgress: (bytesUploaded, bytesTotal) => {
            const percentage = ((bytesUploaded / bytesTotal) * 100).toFixed(0);
            this.progress = +percentage;
          },
        });
      };

      // شروع آپلود
      this.upload = createUpload();

      this.upload.findPreviousUploads().then((previousUploads) => {
        this.uploadedUrl = '';
        this.uploading = true;
        if (previousUploads.length) {
          this.upload?.resumeFromPreviousUpload(previousUploads[0]);
        }
        this.upload?.start();
      });
    });
  }

  // متد کمکی برای پاک کردن localStorage
  private clearTusLocalStorage(): void {
    for (const key in localStorage) {
      if (key.includes('tus')) {
        localStorage.removeItem(key);
      }
    }
  }
  private handle401AndRetry(options: UploadOptions): boolean {
    // const refreshToken = this._tokenService.get().refreshToken ?? '';

    // this.authService.refreshToken(new RefreshTokenCommand({ refreshToken })).subscribe({
    //   next: () => {
    //     options.headers = {
    //       ...options.headers,
    //       Authorization: 'Bearer ' + this._tokenService.get().accessToken,
    //     };
    //     // دوباره تلاش کن
    //     this.upload?.start();
    //   },
    //   error: () => {
    //     this.authService.logout();
    //   },
    // });

    return true; // به tus بگو که retry می‌کنیم
  }
}
