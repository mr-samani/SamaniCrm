import { Injectable } from '@angular/core';
import { PageDto, PagesServiceProxy } from '@shared/service-proxies';
import { UpdatePageContentCommand } from '@shared/service-proxies/model/update-page-content-command';
import { finalize } from 'rxjs/operators';
import { FormBuilderService } from './form-builder.service';
import { AppConst } from '@shared/app-const';
import { BlockDefinition } from '../blocks/block-registry';
import { generateCSSFromBlocks } from '../helpers/generate-css-block';
import { getBlocksAsString } from '../helpers/get-blocks-as-string';
import { NotifyService } from '@shared/services/notify.service';
import { LanguageService } from '@shared/services/language.service';

@Injectable()
export class FormBuilderBackendService {
  loading = false;
  saving = false;
  pageId: string = '';
  pageInfo?: PageDto;

  constructor(
    private pageService: PagesServiceProxy,
    private b: FormBuilderService,
    private notify: NotifyService,
    private language: LanguageService,
  ) {}

  l(key: string, param?: Object) {
    // console.log(this.language.translate.instant(key, param));
    return this.language.translate.instant(key, param);
  }

  getPageInfo() {
    if (!this.pageId) return;
    this.loading = true;
    this.pageService
      .getPageInfo(this.pageId, AppConst.currentLanguage)
      .pipe(
        finalize(() => {
          this.loading = false;
        }),
      )
      .subscribe((response) => {
        this.pageInfo = response.data;
        if (this.pageInfo && this.pageInfo.data) {
          this.b.blocks = this.initBlocks(JSON.parse(this.pageInfo.data ?? '[]'));
          console.log('saved blocks', this.b.blocks);
        }
      });
  }

  private initBlocks(list: BlockDefinition[]) {
    return list.map((item) => new BlockDefinition(item));
  }

  save() {
    const data = getBlocksAsString(this.b.blocks);
    const css = generateCSSFromBlocks(this.b.blocks);

    this.saving = true;
    const input = new UpdatePageContentCommand({
      pageId: this.pageId,
      culture: this.pageInfo?.culture,
      data,
      styles: css,
    });
    this.pageService
      .updatePageContent(input)
      .pipe(
        finalize(() => {
          this.saving  = false;
        }),
      )
      .subscribe(() => {
        this.notify.success(this.l('SavedSuccessfully'));
      });
  }

  preview() {
    if (this.pageInfo)
      window.open(AppConst.publicSiteUrl + '/page/preview/' + this.pageInfo.culture + '/' + this.pageId, '_blank');
  }
}
