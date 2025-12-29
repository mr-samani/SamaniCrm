import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-app-setting',
  templateUrl: './app-setting.component.html',
  styleUrls: ['./app-setting.component.css'],
  standalone: false,
})
export class AppSettingComponent extends AppComponentBase implements OnInit {
  loading = true;

  constructor(injector: Injector) {
    super(injector);
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/panel/setting' },
      { name: this.l('AppSetting'), url: '/panel/app-setting' },
    ];
  }

  ngOnInit(): void {}
}
