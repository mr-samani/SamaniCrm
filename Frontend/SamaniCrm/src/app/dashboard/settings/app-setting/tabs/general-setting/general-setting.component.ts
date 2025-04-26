import { Component, Input, OnInit } from '@angular/core';
import { GeneralSettingDto } from '../../models/app-settings';
import { ControlContainer, NgForm } from '@angular/forms';

@Component({
  selector: 'app-general-setting',
  templateUrl: './general-setting.component.html',
  styleUrls: ['./general-setting.component.css'],
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: NgForm,
    },
  ],
})
export class GeneralSettingComponent implements OnInit {
  @Input() settings?: GeneralSettingDto;
  constructor() {}

  ngOnInit() {}
}
