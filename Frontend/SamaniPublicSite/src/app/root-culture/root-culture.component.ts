import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { BaseComponent } from '@app/base-components';
import { PageViewRoutingModule } from '@app/page-view/page-view-routing.module';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-root-culture',
  templateUrl: './root-culture.component.html',
  styleUrls: ['./root-culture.component.scss'],
  standalone: true,
  imports: [CommonModule, PageViewRoutingModule],
})
export class RootCultureComponent extends BaseComponent implements OnInit {
  cultureParam: string = AppConst.defaultLang;
  constructor(injector: Injector) {
    super(injector);
    this.route.params.subscribe((params) => {
      console.log('set culture:', params['culture']);
      this.cultureParam = params['culture'];
    });
  }

  ngOnInit() {}
}
