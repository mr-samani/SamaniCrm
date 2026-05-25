import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { LogEntryDto } from '@shared/service-proxies/model/log-entry-dto';
import { SharedModule } from '@shared/shared.module';

@Component({
  selector: 'app-log-details',
  templateUrl: './log-details.component.html',
  styleUrls: ['./log-details.component.scss'],
  imports: [CommonModule, MaterialCommonModule, TranslateModule, SharedModule],
})
export class LogDetailsComponent implements OnInit {
  data = inject<LogEntryDto>(MAT_DIALOG_DATA);
  constructor() {}

  ngOnInit() {}
}
