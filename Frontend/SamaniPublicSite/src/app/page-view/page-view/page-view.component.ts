import { AfterViewInit, Component, DOCUMENT, Inject, Injector, OnInit, signal } from '@angular/core';
import { BaseComponent } from '@app/base-components';
import { PageDto, PagesServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';
import { DynamicDataStructure, IPagebuilderOutput, PageBuilderConfig } from 'ngx-page-builder/core';
import { DYNAMIC_DATA } from '../dynamic-data/dynamic-data';

@Component({
  selector: 'app-page-view',
  templateUrl: './page-view.component.html',
  styleUrls: ['./page-view.component.scss'],
  standalone: false,
})
export class PageViewComponent extends BaseComponent implements OnInit, AfterViewInit {
  culture: '';
  slug = '';
  pageId = '';
  loading = signal(false);
  pageInfo?: PageDto;

  data?: IPagebuilderOutput;
  
  dynamicData: DynamicDataStructure[] = DYNAMIC_DATA;
  constructor(
    injector: Injector,
    private pageService: PagesServiceProxy,
    @Inject(DOCUMENT) private _document: Document
  ) {
    super(injector);
    this.culture = this.route.snapshot.params['culture'];
    this.pageId = this.route.snapshot.params['pageId'];
    this.slug = this.route.snapshot.params['slug'];
  }

  ngOnInit() {}
  ngAfterViewInit(): void {
    this.getInfo();
  }

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
        };
        console.log(this.data);

        this.cd.detectChanges();
      });
  }
}
