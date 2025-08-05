import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { BaseComponent } from '@app/base-components';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [CommonModule],
})
export class HomeComponent extends BaseComponent implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
