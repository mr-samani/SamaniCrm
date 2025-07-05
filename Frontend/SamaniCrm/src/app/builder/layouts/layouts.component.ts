import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FormBuilderService } from '../form-builder.service';
import { BlockTypeEnum } from '../blocks/block-registry';

@Component({
  selector: 'layouts',
  templateUrl: './layouts.component.html',
  styleUrls: ['./layouts.component.scss'],
  standalone: false,
})
export class LayoutsComponent extends AppComponentBase implements OnInit {
  open = true;
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
