

import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-create-page-dialog',
  templateUrl: './create-page-dialog.component.html',
  styleUrls: ['./create-page-dialog.component.scss'],
  standalone: false,
  
}) 
export class CreatePageDialogComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
