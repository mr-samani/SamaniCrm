import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LanguagesRoutingModule } from './languages-routing.module';
import { LanguageListComponent } from './language-list/language-list.component';
import { TableViewComponent } from 'src/shared/components/table-view/table-view.compoenent';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [LanguageListComponent],
  imports: [
    CommonModule,
    LanguagesRoutingModule,
    TableViewComponent,
    MatSlideToggleModule,
    TranslateModule,
    FormsModule,
    MatProgressSpinnerModule,
  ],
})
export class LanguagesModule {}
