import { Component,  Input, OnInit } from '@angular/core';
import { ControlContainer, NgForm } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { finalize } from 'rxjs';
import { Apis } from '@shared/apis';
import { MaintenanceServiceProxy } from '@shared/service-proxies/api/maintenance.service';
import { CacheEntryDto } from '@shared/service-proxies';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { humanFileSize } from '@shared/helper/file.helper';

export class CacheEntryDtoExtended extends CacheEntryDto {
  loading?: boolean;
  humanSize?: string;
}

@Component({
  selector: 'app-cache',
  templateUrl: './cache.component.html',
  styleUrls: ['./cache.component.scss'],
  standalone: false,
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: NgForm,
    },
  ],
})
export class CacheComponent extends AppComponentBase implements OnInit {
  cacheKeys: CacheEntryDtoExtended[] = [];
  fields: FieldsType[] = [
    { column: 'key', title: this.l('Key'), type: 'text' },
    { column: 'provider', title: this.l('Provider') },
    { column: 'lastModified', title: this.l('LastModified'), type: 'dateTime' },
    { column: 'expiration', title: this.l('Expiration') },
    { column: 'humanSize', title: this.l('Size'), type: 'text' },
  ];
  totalSize = 0;
  loading = true;
  constructor(
    private maintenanceService: MaintenanceServiceProxy,
  ) {
    super();
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.loading = true;
    this.maintenanceService
      .getAllCacheEntries()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.cacheKeys = response.data ?? [];
        this.totalSize = 0;
        this.cacheKeys.map((m) => {
          this.totalSize += m.sizeInBytes ?? 0;
          m.humanSize = humanFileSize(m.sizeInBytes);
        });
      });
  }

  remove(item: CacheEntryDtoExtended) {
    this.confirmMessage(`${this.l('Delete')}:${item.key}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        item.loading = true;
        this.maintenanceService
          .deleteCache(item.key)
          .pipe(finalize(() => (item.loading = false)))
          .subscribe((response) => {
            this.notify.success(this.l('DoneSuccessFully') + '(' + response.data + ')');
            this.getData();
          });
      }
    });
  }

  clearAll() {
    this.confirmMessage(this.l('Delete'), this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.maintenanceService
          .clearAllCahces()
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            this.notify.success(this.l('DoneSuccessFully') + '(' + response.data + ')');
            this.getData();
          });
      }
    });
  }
}
