import { Component, Injector, OnInit, signal } from '@angular/core';
import { BaseComponent } from '@app/base-components';
import { PageDto, PagesServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-page-view',
  templateUrl: './page-view.component.html',
  styleUrls: ['./page-view.component.scss'],
  standalone: false,
})
export class PageViewComponent extends BaseComponent implements OnInit {
  culture: '';
  slug = '';
  pageId = '';
  loading = signal(false);
  pageInfo?: PageDto;
  constructor(injector: Injector, private pageService: PagesServiceProxy) {
    super(injector);

    this.culture = this.route.snapshot.params['culture'];
    this.pageId = this.route.snapshot.params['pageId'];
    this.slug = this.route.snapshot.params['slug'];
  }

  ngOnInit() {}

  getInfo() {
    this.loading.set(true);
    this.pageService
      .getPageInfo(this.pageId, this.culture)
      .pipe(
        finalize(() => {
          this.loading.set(false);
        })
      )
      .subscribe((result) => {
        this.pageInfo = result.data ?? new PageDto();

        this.cd.detectChanges();
      });
  }
}
