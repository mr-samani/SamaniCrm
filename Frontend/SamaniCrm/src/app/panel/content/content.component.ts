import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { PageTypeEnum } from '@shared/service-proxies';

@Component({
  selector: 'app-content',
  templateUrl: './content.component.html',
  styleUrl: './content.component.scss',
  standalone: false,
})
export class ContentComponent extends AppComponentBase {
  constructor() {
    super();
    this.breadcrumb.list = [{ name: this.l('Content'), url: '/panel/content' }];
  }

  public get PageTypeEnum(): typeof PageTypeEnum {
    return PageTypeEnum;
  }
}
