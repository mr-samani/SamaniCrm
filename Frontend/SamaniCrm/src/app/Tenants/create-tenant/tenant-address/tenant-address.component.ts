import { Component, Injector, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-tenant-address',
  templateUrl: './tenant-address.component.html',
  styleUrls: ['./tenant-address.component.scss'],
  standalone: false,
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: FormGroupDirective,
    },
  ],
})
export class TenantAddressComponent extends AppComponentBase implements OnInit {
  @Input() form!: FormGroup;

  constructor() {
    super();
  }

  ngOnInit() {}
}
