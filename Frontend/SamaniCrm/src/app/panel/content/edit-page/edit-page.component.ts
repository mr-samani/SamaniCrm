import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { PagesServiceProxy } from '@shared/service-proxies/api/pages.service';
import { PageDto } from '@shared/service-proxies/model/page-dto';
import { finalize } from 'rxjs/operators';
import { IPage } from 'ngx-page-builder';

@Component({
  selector: 'app-edit-page',
  templateUrl: './edit-page.component.html',
  styleUrls: ['./edit-page.component.scss'],
  standalone: false,
})
export class EditPageComponent extends AppComponentBase implements OnInit {
  pageId = '';
  pageInfo?: PageDto;
  data: IPage[] = [];
  styles = '';
  constructor(
    injector: Injector,
    private pageService: PagesServiceProxy,
  ) {
    super(injector);
    this.pageId = this.route.snapshot.params['id'];
  }

  ngOnInit() {
    this.getPageInfo();
  }

  getPageInfo() {
    if (!this.pageId) return;
    this.showMainLoading();
    this.pageService
      .getPageInfo(this.pageId, AppConst.currentLanguage)
      .pipe(finalize(() => this.hideMainLoading()))
      .subscribe((response) => {
        this.pageInfo = response.data;
        if (this.pageInfo && this.pageInfo.data) {
          this.data = JSON.parse(this.pageInfo.data ?? '[]');
        }
      });
  }
}
