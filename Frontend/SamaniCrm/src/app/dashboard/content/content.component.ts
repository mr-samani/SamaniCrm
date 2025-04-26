import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-content',
  templateUrl: './content.component.html',
  styleUrl: './content.component.scss',
})
export class ContentComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
    this.breadcrumb.list = [{ name: this.l('Content'), url: '/dashboard/content' }];
  }
}
