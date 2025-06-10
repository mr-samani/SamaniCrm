import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import {
  ActiveOrDeactiveLanguageCommand,
  DeleteLanguageCommand,
  LanguageDTO,
  LanguageServiceProxy,
} from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';
import { LocalizationKeysComponent } from '../localization-keys/localization-keys.component';
import { CreateOrEditLanguageComponent } from '../create-or-edit-language/create-or-edit-language.component';
import { AppConst } from '@shared/app-const';

export class LanguageDTOExtended extends LanguageDTO {
  loading?: boolean;
  direction?: 'RTL' | 'LTR';
}
@Component({
  selector: 'app-language-list',
  templateUrl: './language-list.component.html',
  styleUrls: ['./language-list.component.scss'],
  standalone: false,
})
export class LanguageListComponent extends AppComponentBase implements OnInit {
  loading = true;
  list: LanguageDTOExtended[] = [];
  fields: FieldsType[] = [
    { column: 'flag', title: this.l('Image'), width: 100, type: 'image' },
    { column: 'cultur', title: this.l('Cultur'), width: 100 },
    { column: 'name', title: this.l('Name') },
    { column: 'direction', title: this.l('Direction'), width: 100 },
    { column: 'isDefault', title: this.l('Default'), type: 'yesNo', width: 100 },
    { column: 'isActive', title: this.l('Active'), type: 'yesNo', width: 100 },
  ];
  constructor(
    injector: Injector,
    private matDialog: MatDialog,
    private languageService: LanguageServiceProxy,
  ) {
    super(injector);
    this.breadcrumb.list = [
      { name: this.l('Settings'), url: '/dashboard/setting' },
      { name: this.l('Languages'), url: '/dashboard/languages' },
    ];
  }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.loading = true;
    this.languageService
      .getAllLanguages()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? [];
        this.list.map((x) => {
          x.flag =AppConst.apiUrl +'/images/flags/' + x.flag + '.png';
          x.direction = x.isRtl ? 'RTL' : 'LTR';
        });
      });
  }

  toggleActivate(item: LanguageDTOExtended) {
    item.loading = true;
    const input = new ActiveOrDeactiveLanguageCommand();
    input.culture = item.culture;
    input.isActive = !item.isActive;
    this.languageService
      .activeOrDeactive(input)
      .pipe(finalize(() => (item.loading = false)))
      .subscribe();
  }

  openDetails(item: LanguageDTOExtended) {
    this.matDialog.open(LocalizationKeysComponent, {
      data: {
        culture: item.culture,
      },
    });
  }

  remove(item: LanguageDTOExtended) {
    this.confirmMessage(`${this.l('Delete')}:${item.culture}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        const input = new DeleteLanguageCommand();
        input.culture = item.culture;
        this.languageService
          .deleteLangauuge(input)
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            if (response.success) {
              this.notify.success(this.l('DeletedSuccessfully'));
              this.getList();
            }
          });
      }
    });
  }

  openLanguageDialog(item?: LanguageDTOExtended) {
    this.matDialog
      .open(CreateOrEditLanguageComponent, {
        data: item,
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.getList();
        }
      });
  }
}
