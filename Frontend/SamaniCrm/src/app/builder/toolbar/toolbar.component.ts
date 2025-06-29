

import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss'],
  standalone: false,
  
}) 
export class ToolbarComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
