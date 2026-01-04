import { inject, Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { PageDto } from '@shared/service-proxies/model/page-dto';
import { UpdatePageContentCommand } from '@shared/service-proxies/model/update-page-content-command';
import { IStorageService, PageBuilderDto, PageBuilderService, preparePageDataForSave } from 'ngx-page-builder';
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

  async loadData(): Promise<PageBuilderDto> {
    return new PageBuilderDto();
  }

  async saveData(): Promise<PageBuilderDto> {
    return new Promise(async (resolve, reject) => {
      try {
        if (!this.sharedPageDataService.pageInfo) {
          reject('page not found');
          return;
        }

        const sanitized = await preparePageDataForSave(this.pageBuilder.pageInfo);
        const input = new UpdatePageContentCommand();
        input.data = JSON.stringify(sanitized);
        input.pageId = this.sharedPageDataService.pageInfo.id;
        input.culture = this.sharedPageDataService.pageInfo.culture;
        this.pagesServiceProxy.updatePageContent(input).subscribe((result) => {
          resolve(new PageBuilderDto(sanitized));
        });
      } catch (error) {
        console.error('Error on save page:', error);
        reject(error);
        throw error;
      }
    });
  }
}
