import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SanitizerPipe } from './pipes/sanitizer.pipe';
import { LuxonFormatPipe } from './pipes/luxon-format.pipe';
import { LuxonFromNowPipe } from './pipes/luxon-from-now.pipe';
import { FileSizePipe } from './pipes/file-size.pipe';
import { EnumToArrayPipe, EnumToArrayStringValuePipe } from './pipes/enum-to-array.pipe';
import { NumberCheckDirective } from './directives/number-check.directive';
import { FilteFormrDirective } from './directives/filter-form.directive';
import { PermissionPipe } from './permissions/permission.pipe';
import { PermissionAnyPipe } from './permissions/permission-any.pipe';
import { PermissionAllPipe } from './permissions/permission-all.pipe';

const pipes = [
  SanitizerPipe,
  LuxonFormatPipe,
  LuxonFromNowPipe,
  FileSizePipe,
  EnumToArrayPipe,
  EnumToArrayStringValuePipe,
  NumberCheckDirective,
  FilteFormrDirective,
  PermissionPipe,
  PermissionAnyPipe,
  PermissionAllPipe,
];
@NgModule({
  declarations: [...pipes],
  imports: [CommonModule],
  exports: [...pipes],
})
export class SharedModule {}
