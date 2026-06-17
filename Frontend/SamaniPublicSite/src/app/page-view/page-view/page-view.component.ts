import { AfterViewInit, Component, DOCUMENT, Inject, Injector, OnInit, signal } from '@angular/core';
import { BaseComponent } from '@app/base-components';
import { PageDto, PagesServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';
import { DynamicDataStructure, IPagebuilderOutput, PageBuilderConfig } from 'ngx-page-builder/core';
import { DYNAMIC_DATA } from '../dynamic-data/dynamic-data';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-page-view',
  templateUrl: './page-view.component.html',
  styleUrls: ['./page-view.component.scss'],
  standalone: false,
})
export class PageViewComponent extends BaseComponent implements OnInit, AfterViewInit {
  culture = '';
  slug = '';
  pageId = '';
  loading = signal(false);
  pageInfo?: PageDto;

  data?: IPagebuilderOutput;

  dynamicData: DynamicDataStructure[] = DYNAMIC_DATA;
  constructor(
    injector: Injector,
    private pageService: PagesServiceProxy,
    @Inject(DOCUMENT) private _document: Document,
  ) {
    super(injector);
    this.route.params.subscribe((p) => {
      this.culture = p['culture'] ?? AppConst.currentLanguage;
      this.pageId = p['pageId'];
      this.slug = p['slug'];
      this.getInfo();
    });
  }

  ngOnInit() {}
  ngAfterViewInit(): void {}

  getInfo() {
    this.loading.set(true);
    this.pageService
      .getPageInfo(this.culture, this.pageId, this.slug)
      .pipe(
        finalize(() => {
          this.loading.set(false);
        }),
      )
      .subscribe((result) => {
        console.log(result);
        this.pageInfo = result.data ?? new PageDto();
        const parsed = JSON.parse(this.pageInfo.data ?? '{}');
        let styles = [];
        try {
          styles = JSON.parse(this.pageInfo?.styles ?? '[]');
        } catch (error) {
          console.warn('Error on parse styles:', error);
          styles = [];
        }

        this.data = {
          config: new PageBuilderConfig(),
          data: parsed,
          styles: styles,
          cssVariables: [],
        };
        console.log('pagebuilderData', this.data);

        this.cd.detectChanges();
      });
  }
}
