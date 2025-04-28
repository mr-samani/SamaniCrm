import { ErrorHandler, Injectable, Injector, NgZone } from '@angular/core';
import { NgxAlertModalService } from 'ngx-alert-modal';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    private zone: NgZone,
    private injector: Injector,
  ) {}

  handleError(error: any): void {
    const errorMessage = error?.message || '';
    const fetchFileErrors = [
      // Chrome, Edge, Opera (Desktop & Mobile)
      'Failed to fetch dynamically imported module',

      // Firefox (Desktop & Mobile)
      'error loading dynamically imported module',

      // Safari (Desktop & Mobile)
      'Load failed for the module script with source',

      // Older iOS Safari (برخی نسخه‌های قدیمی‌تر)
      'Importing a module script failed',

      // Brave browser (similar to Chrome sometimes)
      'Failed to load module script',
      // General fallback (برای بعضی مرورگرهای خاص یا نسخه‌های خاص موبایل)
      'ChunkLoadError',
    ];

    if (fetchFileErrors.findIndex((x) => errorMessage.includes(x)) > -1) {
      console.error('❌ Chunk load error detected. Reloading the page...');
      const alertService = this.injector.get(NgxAlertModalService);
      alertService
        .show({
          title: 'صفحه مورد نظر آپدیت شده است!',
          html: `
          <div class="alert alert-sm alert-danger">لطفاً ابتدا مرورگر خود را رفرش کنید، سپس مجدداً وارد لینک مورد نظر شوید.</div>
          <div class="alert alert-sm alert-warning">در صورتی که مجددا با این پیام روبرو شدید لطفاً کش مرورگر خود را خالی کنید. </div>
          `,
          showConfirmButton: true,
          showCancelButton: true,
          allowEnterKey: true,
          allowOutsideClick: false,
          cancelButtonText: 'انصراف',
          confirmButtonText: 'بروز رسانی',
        })
        .then((r) => {
          if (r.isConfirmed) {
            this.zone.run(() => {
              location.reload();
            });
          }
        });
      return;
    }
    console.error('❗An error occurred:', error);
  }
}
