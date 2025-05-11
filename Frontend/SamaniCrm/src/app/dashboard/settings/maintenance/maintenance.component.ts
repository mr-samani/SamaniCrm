import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-maintenance',
  templateUrl: './maintenance.component.html',
  styleUrls: ['./maintenance.component.scss'],
  standalone: false,
})
export class MaintenanceComponent extends AppComponentBase implements OnInit {
  loading = true;

  isSaving = false;

  constructor(injector: Injector) {
    super(injector);

    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/dashboard/setting' },
      { name: this.l('Maintenance'), url: '/dashboard/maintenance' },
    ];
  }

  ngOnInit(): void {
  }
}
