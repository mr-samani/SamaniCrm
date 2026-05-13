import { Component, Injector, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-tenant-admin-user',
  templateUrl: './tenant-admin-user.component.html',
  styleUrls: ['./tenant-admin-user.component.scss'],
  standalone: false,
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: FormGroupDirective,
    },
  ],
})
export class TenantAdminUserComponent extends AppComponentBase implements OnInit {
  @Input() form!: FormGroup;
  constructor() {
    super();
  }

  ngOnInit() {}
}
