import { inject, Injector } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Meta, Title } from '@angular/platform-browser';
import { AuthService } from '@shared/services/auth.service';
import { MainSpinnerService } from '@shared/services/main-spinner.service';
import { NotifyService } from '@shared/services/notify.service';
import { LanguageService } from '@shared/services/language.service';
import { NgxAlertModalService } from 'ngx-alert-modal';
import { BreadcrumbService } from '@shared/services/breadcrumb.service';
import { DashboardService } from './dashboard/dashboard/dashboard.service';
import { DOCUMENT } from '@angular/common';
export abstract class AppComponentBase {
  fb: FormBuilder;
  authService: AuthService;
  router: Router;
  route: ActivatedRoute;
  browserTitle: Title;
  metaTag: Meta;
  notify: NotifyService;
  private mainSpinnerService: MainSpinnerService;
  language: LanguageService;
  alert: NgxAlertModalService;
  breadcrumb: BreadcrumbService;

  dashboardService: DashboardService;

  doc: Document;
  constructor(injector: Injector) {
    this.mainSpinnerService = injector.get(MainSpinnerService);
    this.authService = injector.get(AuthService);
    this.fb = injector.get(FormBuilder);
    this.router = injector.get(Router);
    this.route = injector.get(ActivatedRoute);
    this.browserTitle = injector.get(Title);
    this.metaTag = injector.get(Meta);
    this.notify = injector.get(NotifyService);
    this.language = injector.get(LanguageService);
    this.alert = injector.get(NgxAlertModalService);
    this.breadcrumb = injector.get(BreadcrumbService);
    this.dashboardService = injector.get(DashboardService);
    this.doc = injector.get(DOCUMENT) as Document;
  }

  l(key: string, param?: Object) {
    // console.log(this.language.translate.instant(key, param));
    return this.language.translate.instant(key, param);
  }

  showMainLoading() {
    this.mainSpinnerService.showLoading = true;
  }

  hideMainLoading() {
    this.mainSpinnerService.showLoading = false;
  }

  confirmMessage(title: string, text: string): Promise<import('ngx-alert-modal').AlertResult<any>> {
    return this.alert.show({
      title: title,
      text: text,
      showConfirmButton: true,
      confirmButtonText: this.l('Ok'),
      showCancelButton: true,
      cancelButtonText: this.l('Cancel'),
    });
  }
}
