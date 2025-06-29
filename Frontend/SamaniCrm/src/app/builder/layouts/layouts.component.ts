

import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'layouts',
  templateUrl: './layouts.component.html',
  styleUrls: ['./layouts.component.scss'],
  standalone: false,
  
}) 
export class LayoutsComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
