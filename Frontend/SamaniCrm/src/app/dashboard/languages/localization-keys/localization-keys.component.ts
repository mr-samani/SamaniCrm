import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { PageEvent } from '@shared/components/pagination/pagination.component';
import {
  DeleteLocalizeKeyCommand,
  LocalizationCategoryEnum,
  LocalizationKeyDTO,
  UpdateBatchLocalizeKeyCommand,
} from '@shared/service-proxies';
import { LanguageServiceProxy } from '@shared/service-proxies/api/language.service';
import { DownloadService } from '@shared/services/download.service';
import { JsonFileReaderService } from '@shared/services/json-file-reader.service';
import { cloneDeep, toLower } from 'lodash-es';
import { finalize } from 'rxjs/operators';
import { AddNewLocalizeKeyComponent } from '../add-new-localize-key/add-new-localize-key.component';

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
  allLocalizations: LocalizationKeyDTO[] = [];
  filteredList: LocalizationKeyDTO[] = [];
  totalCount = 0;
  filterKey: string = '';
  filterValue: string = '';
  page = 1;
  perPage = 10;

  busy = false;

  importCategory = LocalizationCategoryEnum.Other;
  filterCategory?: LocalizationCategoryEnum;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: { culture: string },
    private matDialogRef: MatDialogRef<LocalizationKeysComponent>,
    private languageService: LanguageServiceProxy,
    private downloadService: DownloadService,
    private jsonFileReaderService: JsonFileReaderService,
    private matDialog: MatDialog,
  ) {
    super(injector);
    this.culture = _data.culture;
  }

  ngOnInit() {
    this.getList();
  }

  public get LocalizationCategoryEnum(): typeof LocalizationCategoryEnum {
    return LocalizationCategoryEnum;
  }

  getList() {
    this.loading = true;
    this.languageService
      .getAllLanguageKeys(this.culture)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.allLocalizations = response.data ?? [];
        this.search();
      });
  }

  search(ev?: PageEvent, data = this.allLocalizations) {
    if (!ev) this.page = 1;
    const filtered = this.allLocalizations.filter((x) => {
      let c = [];
      if (this.filterCategory != undefined) {
        c.push(x.category === this.filterCategory);
      }
      if (this.filterKey != '') {
        c.push(x.key.toLowerCase().includes(this.filterKey.toLowerCase()));
      }
      if (this.filterValue != '') {
        c.push(x.value?.toLowerCase().includes(this.filterValue.toLowerCase()));
      }
      return c.every(Boolean);
    });

    const from = (this.page - 1) * this.perPage;
    const to = from + this.perPage;
    this.filteredList = filtered.slice(from, to);
    this.totalCount = filtered.length;
  }

  save() {
    this.saving = true;
    const input = new UpdateBatchLocalizeKeyCommand();
    input.culture = this.culture;
    input.data = this.allLocalizations;
    this.languageService
      .updateBatchLocalizeKey(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success('SavedSuccessfully');
          this.getList();
        }
      });
  }

  remove(key: string, index: number) {
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
              this.allLocalizations.splice(index, 1);
              this.search();
            }
          });
      }
    });
  }

  exportData() {
    this.busy = true;
    const data: { [key: string]: string } = {};
    for (let item of this.allLocalizations) {
      data[item.key] = item.value ?? '';
    }
    this.downloadService
      .generateDownloadJson(data, 'localization_' + this.culture + '.json')
      .then((_) => (this.busy = false));
  }

  importData() {
    this.jsonFileReaderService.selectAndReadJson().then((data: { [key: string]: string }) => {
      console.table(data);
      try {
        for (let item of Object.entries(data)) {
          const found = this.allLocalizations.find((x) => x.key == item[0] && x.category == this.importCategory);
          if (found && item[1]) {
            found.value = item[1];
          }
          if (!found) {
            this.allLocalizations.push(
              new LocalizationKeyDTO({
                category: this.importCategory,
                culture: this.culture,
                key: item[0],
                value: item[1],
              }),
            );
          }
        }
        this.filterKey = '';
        this.filterValue = '';
        this.filterCategory = undefined;
        this.search();
      } catch (e) {
        this.notify.error(this.l('JsonFileIsInvalid'));
      }
    });
  }

  openNewLocalizeDialog() {
    this.matDialog
      .open(AddNewLocalizeKeyComponent, {
        data: {
          culture: this.culture,
        },
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.getList();
        }
      });
  }
}
