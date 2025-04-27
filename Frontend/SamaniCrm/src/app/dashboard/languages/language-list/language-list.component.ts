import { Component, Injector, OnInit } from '@angular/core';
import { Apis } from '@shared/apis';
import { LanguageDto } from '@shared/models/language-dto';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from 'src/app/app-component-base';
import { FieldsType } from 'src/shared/components/table-view/fields-type.model';

@Component({
  selector: 'app-language-list',
  templateUrl: './language-list.component.html',
  styleUrls: ['./language-list.component.scss'],
})
export class LanguageListComponent extends AppComponentBase implements OnInit {
  loading = true;
  list: LanguageDto[] = [];
  fields: FieldsType[] = [
    { column: 'image', title: this.l('Image'), width: 100, type: 'image' },
    { column: 'code', title: this.l('Code'), width: 100 },
    { column: 'name', title: this.l('Name') },
    { column: 'title', title: this.l('Title') },
  ];
  constructor(injector: Injector) {
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
    this.dataService
      .get<any, LanguageDto[]>(Apis.getAllLanguages, {})
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.list = response.data ?? [];
      });
  }

  changeActive(item: LanguageDto) {
    item.loading = true;
    this.dataService
      .post<any, LanguageDto[]>(Apis.changeActiveLanguage, {
        lang: item.code,
        isActive: !item.isActive,
      })
      .pipe(finalize(() => (item.loading = false)))
      .subscribe();
  }
}
