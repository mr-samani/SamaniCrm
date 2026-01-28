import { Component,  ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-panel',
  templateUrl: './panel.component.html',
  styleUrls: ['./panel.component.scss'],
  encapsulation: ViewEncapsulation.None,
  standalone: false,
})
export class PanelComponent extends AppComponentBase {
  AppConst = AppConst;
  constructor() {
    super();
  }
}
