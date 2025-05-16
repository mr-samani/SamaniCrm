import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  breadcrumb = {
    enable: signal(true),
  };

  set showBreadCrumb(v: boolean) {
    this.breadcrumb.enable.set(v);
  }
}
