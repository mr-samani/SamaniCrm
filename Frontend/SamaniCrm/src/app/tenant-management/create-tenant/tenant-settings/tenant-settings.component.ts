import { Component, Injector, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { DatabaseStrategy } from '@shared/service-proxies/model/database-strategy';

@Component({
  selector: 'app-tenant-settings',
  templateUrl: './tenant-settings.component.html',
  styleUrls: ['./tenant-settings.component.scss'],
  standalone: false,
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: FormGroupDirective,
    },
  ],
})
export class TenantSettingsComponent extends AppComponentBase implements OnInit {
  @Input() form!: FormGroup;
  constructor() {
    super();
  }

  ngOnInit() {}

  public get DatabaseStrategy(): typeof DatabaseStrategy {
    return DatabaseStrategy;
  }
}
