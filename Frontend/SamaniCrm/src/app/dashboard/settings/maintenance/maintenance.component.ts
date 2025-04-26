import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { Apis } from '@shared/apis';
import { finalize } from 'rxjs';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-maintenance',
  templateUrl: './maintenance.component.html',
  styleUrls: ['./maintenance.component.css'],
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
    // this.getSettings();
  }

  // getSettings() {
  //   this.loading = true;
  //   this.dataService.get<any, MaintenancesDto>(Apis.getAllSettings, {})
  //     .pipe(finalize(() => this.loading = false))
  //     .subscribe(response => {
  //       this.settings = response.result;
  //     });
  // }

  // save(frm: NgForm) {
  //   if (frm.invalid || !this.settings) {
  //     this.notify.warning(this.l('CompleteFormFields'));
  //     return;
  //   }

  //   this.isSaving = true;
  //   this.dataService.post<{ settings: MaintenancesDto }, null>(
  //     Apis.saveSettings,
  //     { settings: this.settings }
  //   )
  //     .pipe(finalize(() => this.isSaving = false))
  //     .subscribe(response => {
  //       this.notify.success(this.l('SaveSuccessFully'));
  //       this.getSettings();
  //     });
  // }
}
