

import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'properties',
  templateUrl: './properties.component.html',
  styleUrls: ['./properties.component.scss'],
  standalone: false,
  
}) 
export class PropertiesComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
