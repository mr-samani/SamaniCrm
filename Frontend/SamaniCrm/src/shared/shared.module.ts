import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SanitizerPipe } from './pipes/sanitizer.pipe';
import { LuxonFormatPipe } from './pipes/luxon-format.pipe';
import { LuxonFromNowPipe } from './pipes/luxon-from-now.pipe';
import { FileSizePipe } from './pipes/file-size.pipe';
import { EnumToArrayPipe, EnumToArrayStringValuePipe } from './pipes/enum-to-array.pipe';
import { NumberCheckDirective } from './directives/number-check.directive';

const pipes = [
  SanitizerPipe,
  LuxonFormatPipe,
  LuxonFromNowPipe,
  FileSizePipe,
  EnumToArrayPipe,
  EnumToArrayStringValuePipe,
  NumberCheckDirective,
];
@NgModule({
  declarations: [...pipes],
  imports: [CommonModule],
  exports: [...pipes],
})
export class SharedModule {}
