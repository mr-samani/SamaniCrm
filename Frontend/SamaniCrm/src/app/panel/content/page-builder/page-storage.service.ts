import { Injectable } from '@angular/core';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { UpdatePageContentCommand } from '@shared/service-proxies/model/update-page-content-command';
import { IPagebuilderOutput, IStorageService, PageBuilderService, preparePageDataForSave } from 'ngx-page-builder';
import { SharedPageDataService } from './shared-page-data.service';

@Injectable({
  providedIn: 'root',
})
export class PageStorageService implements IStorageService {
  constructor(
    private pageBuilder: PageBuilderService,
    private sharedPageDataService: SharedPageDataService,
    private pagesServiceProxy: PagesServiceProxy,
  ) {}

  async loadData(): Promise<IPagebuilderOutput> {
    throw Error('Not implemented');
  }

  async saveData(): Promise<boolean> {
    return new Promise(async (resolve, reject) => {
      try {
        if (!this.sharedPageDataService.pageInfo) {
          reject('page not found');
          return;
        }

        const sanitized = await preparePageDataForSave(this.pageBuilder);
        const input = new UpdatePageContentCommand();
        input.data = JSON.stringify(sanitized.data);
        input.pageId = this.sharedPageDataService.pageInfo.id;
        input.culture = this.sharedPageDataService.pageInfo.culture;
        input.styles = JSON.stringify(sanitized.styles);
        this.pagesServiceProxy.updatePageContent(input).subscribe((r) => resolve(r.success == true));
      } catch (error) {
        console.error('Error on save page:', error);
        reject(error);
        throw error;
      }
    });
  }
}
