import { Component, Injector, Input, OnInit } from '@angular/core';
import { ControlContainer, NgForm } from '@angular/forms';
import { CacheKey } from '../../models/maintenance';
import { AppComponentBase } from '@app/app-component-base';
import { finalize } from 'rxjs';
import { Apis } from '@shared/apis';

@Component({
  selector: 'app-cache',
  templateUrl: './cache.component.html',
  styleUrls: ['./cache.component.css'],
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: NgForm,
    },
  ],
})
export class CacheComponent extends AppComponentBase implements OnInit {
  cacheKeys: CacheKey[] = [];
  loading = true;
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.loading = true;
    // this.dataService
    //   .get<any, CacheKey[]>(Apis.getAllCacheKeys, {})
    //   .pipe(finalize(() => (this.loading = false)))
    //   .subscribe((response) => {
    //     this.cacheKeys = response.data ?? [];
    //   });
  }

  clear(key: CacheKey) {
    if (!key) {
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }

    key.loading = true;
    // this.dataService
    //   .post(Apis.clearCache, { key: key.key })
    //   .pipe(finalize(() => (key.loading = false)))
    //   .subscribe((response) => {
    //     this.notify.success(this.l('DoneSuccessFully') + '(' + response.data + ')');
    //   });
  }
}
