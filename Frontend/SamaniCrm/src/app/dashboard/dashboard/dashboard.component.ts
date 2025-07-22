import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  encapsulation: ViewEncapsulation.None,
  standalone: false,
})
export class DashboardComponent extends AppComponentBase {
  AppConst = AppConst;
  constructor(injector: Injector) {
    super(injector);
  }
}
