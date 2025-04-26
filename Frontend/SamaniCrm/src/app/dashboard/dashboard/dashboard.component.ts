import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from 'src/app/app-component-base';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class DashboardComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
