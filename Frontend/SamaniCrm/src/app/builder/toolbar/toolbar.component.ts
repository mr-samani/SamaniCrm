import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { ViewModeEnum } from '../models/view-mode.enum';
import { FormBuilderService } from '../form-builder.service';
import { FormBuilderBackendService } from '../backend.service';

@Component({
  selector: 'toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss'],
  standalone: false,
})
export class ToolbarComponent extends AppComponentBase implements OnInit {
  constructor(
    public b: FormBuilderService,
    public backendService: FormBuilderBackendService,
    injector: Injector,
  ) {
    super(injector);
  }

  ngOnInit() {}

  public get ViewModeEnum(): typeof ViewModeEnum {
    return ViewModeEnum;
  }
}
