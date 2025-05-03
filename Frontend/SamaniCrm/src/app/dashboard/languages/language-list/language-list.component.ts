import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { ActiveOrDeactiveLanguageCommand, LanguageDTO, LanguageServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';

export class LanguageDTOExtended extends LanguageDTO {
  loading?: boolean;
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
    { column: 'isRtl', title: this.l('Rtl'), type: 'yesNo', width: 100 },
    { column: 'isDefault', title: this.l('Default'), type: 'yesNo', width: 100 },
    { column: 'isActive', title: this.l('Active'), type: 'yesNo', width: 100 },
  ];
  constructor(
    injector: Injector,

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
        this.list.map((x) => (x.flag = 'images/flags/' + x.flag + '.png'));
      });
  }

  changeActive(item: LanguageDTOExtended) {
    item.loading = true;
    const input = new ActiveOrDeactiveLanguageCommand();
    input.culture = item.culture;
    input.isActive = !item.isActive;
    this.languageService
      .activeOrDeactive(input)
      .pipe(finalize(() => (item.loading = false)))
      .subscribe();
  }
}
