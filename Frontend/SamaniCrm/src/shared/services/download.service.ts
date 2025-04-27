import { Injectable, Injector } from '@angular/core';
import { TokenService } from './token.service';
import { HttpClient } from '@angular/common/http';
import { AppComponentBase } from '@app/app-component-base';

export enum DownloadFileType {
  Excel = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
}

@Injectable({
  providedIn: 'root',
})
export class DownloadService extends AppComponentBase {
  constructor(
    injector: Injector,
    private tokenService: TokenService,
    private http: HttpClient,
  ) {
    super(injector);
  }

  downloadUrlWithToken(url: string, fileType: DownloadFileType, fileName: string) {
    let pw = this.notify.info(this.l('PleaseWait'));

    const headers = {
      Authorization: 'Bearer ' + this.tokenService.get().accessToken,
    };
    this.http
      .get(url, {
        responseType: 'arraybuffer',
        headers: headers,
      })
      .subscribe((result) => {
        this.downLoadFile(result, fileType, fileName).finally(() => pw.dismiss());
      });
  }

  /**
   * Method is use to download file.
   * @param data - Array Buffer data
   * @param type - type of the document.
   */
  private downLoadFile(data: BlobPart, type: string, fileName: string): Promise<boolean> {
    return new Promise((resolve, reject) => {
      try {
        let blob = new File([data], fileName, { type: type, endings: 'native' });
        let url = window.URL.createObjectURL(blob);

        var a: any = document.createElement('a');
        document.body.appendChild(a);
        a.style = 'display: none';
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();
        resolve(true);
        // let openInNewTab = '_self';
        // if (!this.platform.ANDROID && !this.platform.IOS) {
        //     openInNewTab = '_blank';
        // }
        // let pwa = window.open(url, openInNewTab);
        // if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
        //     Swal.fire({
        //         text: this.localizationService.localize('ClickLinkBelowToDownloadFile', this.localizationSourceName),
        //         showCancelButton: false,
        //         showConfirmButton: true,
        //         confirmButtonText: this.localizationService.localize('FileDownload', this.localizationSourceName)
        //     }).then(res => {
        //         if (res.isConfirmed) {
        //             window.open(url, openInNewTab);
        //         }
        //     });
        // }
      } catch (error) {
        reject(error);
      }
    });
  }

  downloadFileFromServer(url: string, fileName: string) {
    this.notify.info(this.l('PleaseWait'));
    var a: any = document.createElement('a');
    document.body.appendChild(a);
    a.style = 'display: none';
    a.href = url;
    a.download = fileName;
    a.click();
    window.URL.revokeObjectURL(url);
    a.remove();
  }

  generateDownloadJson(jsonData: any, fileName: string): Promise<void> {
    return new Promise((resolve, reject) => {
      try {
        let v = this.notify.info(this.l('PleaseWait'));
        var theJSON = JSON.stringify(jsonData);
        var uri = 'data:text/json;charset=UTF-8,' + encodeURIComponent(theJSON);
        var a: any = document.createElement('a');
        document.body.appendChild(a);
        a.style = 'display: none';
        a.href = uri;
        a.download = fileName;
        a.click();
        a.remove();
        setTimeout(() => {
          v.dismiss();
        }, 3000);
        resolve();
      } catch (error) {
        reject(error);
      }
    });
  }
}
