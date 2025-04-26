import { Component, Injector } from '@angular/core';
import { AppComponentBase } from 'src/app/app-component-base';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss'],
})
export class FooterComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
