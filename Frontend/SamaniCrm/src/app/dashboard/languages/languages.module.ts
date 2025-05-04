import { LocalizationKeysComponent } from './localization-keys/localization-keys.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LanguagesRoutingModule } from './languages-routing.module';
import { LanguageListComponent } from './language-list/language-list.component';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableViewComponent } from '@shared/components/table-view/table-view.compoenent';
import { LanguageServiceProxy } from '@shared/service-proxies/api/language.service';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { CreateOrEditLanguageComponent } from './create-or-edit-language/create-or-edit-language.component';

@NgModule({
  declarations: [LanguageListComponent, LocalizationKeysComponent, CreateOrEditLanguageComponent],
  imports: [
    CommonModule,
    LanguagesRoutingModule,
    TableViewComponent,
    TranslateModule,
    FormsModule,
    MaterialCommonModule,
    PaginationComponent,
    ReactiveFormsModule,
  ],
  providers: [LanguageServiceProxy],
})
export class LanguagesModule {}
