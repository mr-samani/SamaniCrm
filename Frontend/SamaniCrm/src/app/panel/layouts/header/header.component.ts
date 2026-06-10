import { Component, ElementRef, ViewChild } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
declare var bootstrap: any;

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: false,
})
export class HeaderComponent extends AppComponentBase {
  @ViewChild('navbarCollapse') navbarCollapse!: ElementRef;

  constructor() {
    super();
  }

  closeNavbar() {
    const collapse = new bootstrap.Collapse(this.navbarCollapse.nativeElement, {
      toggle: false,
    });
    collapse.hide();
  }
}
