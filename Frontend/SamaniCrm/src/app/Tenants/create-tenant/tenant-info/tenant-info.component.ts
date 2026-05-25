import { Component, Injector, Input, input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-tenant-info',
  templateUrl: './tenant-info.component.html',
  styleUrls: ['./tenant-info.component.scss'],
  standalone: false,
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: FormGroupDirective,
    },
  ],
})
export class TenantInfoComponent extends AppComponentBase implements OnInit {
  @Input() form!: FormGroup;

  constructor() {
    super();
  }

  ngOnInit() {}
}
