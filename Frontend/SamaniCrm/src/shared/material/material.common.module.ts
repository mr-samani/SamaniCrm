import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { TranslateService } from '@ngx-translate/core';
import { MaterialJalaliDateAdapter, PERSIAN_DATE_FORMATS } from './jalali/material-jalali-date-adapter';
import {
  MAT_LUXON_DATE_FORMATS,
  LuxonDateAdapter,
  MAT_LUXON_DATE_ADAPTER_OPTIONS,
} from '@angular/material-luxon-adapter';
import { MAT_DATE_LOCALE, MAT_DATE_FORMATS, DateAdapter } from '@angular/material/core';
import { MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { CdkDrag, CdkDragPlaceholder, CdkDropList, DragDropModule } from '@angular/cdk/drag-drop';
import { MatSelectModule } from '@angular/material/select';
import { BidiModule } from '@angular/cdk/bidi';
import { DragPreviewRtlDirective } from './drag-preview-rtl.directive';

export function getLanguage(localizationService: TranslateService) {
  return localizationService.currentLang;
}
export function getDateFormat(locale: string) {
  if (locale === 'fa-IR') {
    return PERSIAN_DATE_FORMATS;
  } else {
    return MAT_LUXON_DATE_FORMATS;
  }
}
export function getDateProvider(locale: string) {
  if (locale === 'fa-IR') {
    return new MaterialJalaliDateAdapter(locale);
  } else {
    return new LuxonDateAdapter(locale);
  }
}

const shared = [
  DragPreviewRtlDirective,
  MatFormFieldModule,
  MatInputModule,
  MatCheckboxModule,
  MatRadioModule,
  MatButtonModule,
  MatDialogModule,
  MatMenuModule,
  MatProgressSpinnerModule,
  MatSlideToggleModule,
  MatProgressBarModule,
  MatIconModule,
  DragDropModule,
  CdkDropList,
  CdkDrag,
  CdkDragPlaceholder,
  MatSelectModule,
];
@NgModule({
  imports: [...shared],
  exports: [...shared],
  providers: [
    { provide: MAT_LUXON_DATE_ADAPTER_OPTIONS, useValue: { useUtc: false } },
    {
      provide: MAT_DATE_LOCALE,
      useFactory: getLanguage,
      deps: [TranslateService],
    },
    {
      provide: MAT_DATE_FORMATS,
      useFactory: getDateFormat,
      deps: [MAT_DATE_LOCALE],
    },
    {
      provide: DateAdapter,
      useFactory: getDateProvider,
      deps: [MAT_DATE_LOCALE],
    },
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: {
        hasBackdrop: true,
        disableClose: true,
        panelClass: 'custom-dialog',
      },
    },
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: { appearance: 'outline' },
    },
  ],
})
export class MaterialCommonModule {}
