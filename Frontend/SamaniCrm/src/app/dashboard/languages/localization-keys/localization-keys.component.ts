import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import { DeleteLocalizeKeyCommand } from '@shared/service-proxies';
import { LanguageServiceProxy } from '@shared/service-proxies/api/language.service';
import { DownloadService } from '@shared/services/download.service';
import { JsonFileReaderService } from '@shared/services/json-file-reader.service';
import { cloneDeep, toLower } from 'lodash-es';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-localization-keys',
  templateUrl: './localization-keys.component.html',
  styleUrls: ['./localization-keys.component.scss'],
  standalone: false,
})
export class LocalizationKeysComponent extends AppComponentBase implements OnInit {
  culture = '';
  loading = true;
  saving = false;
  allLocalizations: {
    [key: string]: string | null;
  } = {};
  filteredList: {
    [key: string]: string | null;
  } = {};
  totalCount = 0;
  filter: string = '';
  page = 1;
  perPage = 10;

  busy = false;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: { culture: string },
    private matDialogRef: MatDialogRef<LocalizationKeysComponent>,
    private languageService: LanguageServiceProxy,
    private downloadService: DownloadService,
    private jsonFileReaderService: JsonFileReaderService,
  ) {
    super(injector);
    this.culture = _data.culture;
  }

  ngOnInit() {
    this.getList();
  }

  getList() {
    this.loading = true;
    this.languageService
      .getAllLanguageKeys(this.culture)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.allLocalizations = response.data ?? {};
        this.search();
      });
  }

  search(ev?: PageEvent, data = this.allLocalizations) {
    if (!ev) this.page = 1;
    const filtered = Object.keys(this.allLocalizations).reduce(
      (filtered: { [key: string]: string | null }, key: string) => {
        if (
          // search value
          this.allLocalizations[key]?.toLowerCase().includes(this.filter.toLowerCase()) ||
          // search key
          key?.toLowerCase().includes(this.filter.toLowerCase())
        ) {
          filtered[key] = this.allLocalizations[key];
        }
        return filtered;
      },
      {},
    );

    const from = (this.page - 1) * this.perPage;
    const to = from + this.perPage;
    this.filteredList = Object.fromEntries(Object.entries(filtered).slice(from, to));
    this.totalCount = Object.entries(filtered).length;
  }

  save() {}

  remove(key: string) {
    this.confirmMessage(`${this.l('Delete')}:${key}`, this.l('ThisKeyWasDeletedFromAllLanguages')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        const input = new DeleteLocalizeKeyCommand();
        input.key = key;
        this.languageService
          .deleteKey(input)
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            if (response.success) {
              this.notify.success(this.l('DeletedSuccessfully'));
              delete this.allLocalizations[key];
              this.search();
            }
          });
      }
    });
  }

  exportData() {
    this.busy = true;
    this.downloadService
      .generateDownloadJson(this.allLocalizations, 'localization_' + this.culture + '.json')
      .then((_) => (this.busy = false));
  }

  importData() {
    this.jsonFileReaderService.selectAndReadJson().then((data: { [key: string]: string }) => {
      console.table(data);
      try {
        for (let item of Object.entries(data)) {
          if (this.allLocalizations[item[0]] !== undefined && item[1]) {
            this.allLocalizations[item[0]] = item[1];
          }
        }
        this.filter = '';
        this.search();
      } catch (e) {
        this.notify.error(this.l('JsonFileIsInvalid'));
      }
    });
  }
}
