import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss'],
  standalone: false,
})
export class FooterComponent extends AppComponentBase {
  constructor() {
    super();
  }
}
