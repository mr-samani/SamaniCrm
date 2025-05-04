import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { NotifyService } from './notify.service';

@Injectable({
  providedIn: 'root',
})
export class JsonFileReaderService {
  constructor(
    private notify: NotifyService,
    private translateService: TranslateService,
  ) {}
  l(key: string, param?: Object) {
    return this.translateService.instant(key, param);
  }

  selectAndReadJson(): Promise<any> {
    return new Promise((resolve, reject) => {
      const input = document.createElement('input');
      input.type = 'file';
      input.accept = 'application/json';

      input.onchange = (event: any) => {
        const file: File = event.target.files[0];
        if (!file) {
          this.notify.warning(this.l('NotSelectedAnyFile'));
          reject(null);
          return;
        }

        const reader = new FileReader();
        reader.onload = () => {
          try {
            const json = JSON.parse(reader.result as string);
            resolve(json);
          } catch (e) {
            this.notify.error(this.l('JsonFileIsInvalid'));
            console.error('Error on read JSON: ' + e);
            reject(null);
          }
        };

        reader.onerror = (e) => {
          this.notify.error(this.l('JsonFileIsInvalid'));
          console.error('Error on read JSON: ' + e);
          reject(null);
        };

        reader.readAsText(file);
      };

      input.click();
    });
  }
}
