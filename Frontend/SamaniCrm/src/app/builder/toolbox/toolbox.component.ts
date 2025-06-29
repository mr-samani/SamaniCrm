import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { BlockTypeEnum } from '../blocks/block-registry';
import { FormBuilderService } from '../form-builder.service';

@Component({
  selector: 'toolbox',
  templateUrl: './toolbox.component.html',
  styleUrls: ['./toolbox.component.scss'],
  standalone: false,
})
export class ToolboxComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    public b: FormBuilderService,
  ) {
    super(injector);
  }

  ngOnInit() {}

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }
}
