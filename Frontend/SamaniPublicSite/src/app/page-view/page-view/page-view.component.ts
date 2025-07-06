import { AfterViewInit, Component, Injector, OnInit, signal, ViewChild, ViewContainerRef } from '@angular/core';
import { BaseComponent } from '@app/base-components';
import { PageDto, PagesServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';
import { FormBuilderService } from '../form-builder.service';
import { BLOCK_REGISTRY, BlockDefinition } from '../blocks/block-registry';
import { CreateBlock } from '../helper/create-block';

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
  @ViewChild('container', { read: ViewContainerRef, static: false }) vcr?: ViewContainerRef;
  constructor(injector: Injector, private pageService: PagesServiceProxy, public b: FormBuilderService) {
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
        const pageData = this.initBlocks(JSON.parse(this.pageInfo.data ?? '[]'));
        console.log(pageData);
        this.renderNode(pageData);
        this.cd.detectChanges();
      });
  }

  initBlocks(list: BlockDefinition[]) {
    for (let item of list) {
      if (item.hidden) {
        continue;
      }
      item = new BlockDefinition(item);
      if (item.children && item.children.length) {
        this.initBlocks(item.children);
      }
    }
    return list;
  }

  renderNode(list: BlockDefinition[]) {
    if (!this.vcr) return;
    for (let item of list) {
      CreateBlock(this.vcr, item);

      // if (item.children && item.children.length) {
      //   this.renderNode(item.children);
      // }
    }
  }
}
