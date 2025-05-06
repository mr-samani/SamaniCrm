import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SanitizerPipe } from './pipes/sanitizer.pipe';
import { LuxonFormatPipe } from './pipes/luxon-format.pipe';
import { LuxonFromNowPipe } from './pipes/luxon-from-now.pipe';
import { FileSizePipe } from './pipes/file-size.pipe';
import { EnumToArrayPipe } from './pipes/enum-to-array.pipe';

const pipes = [SanitizerPipe, LuxonFormatPipe, LuxonFromNowPipe, FileSizePipe, EnumToArrayPipe];
@NgModule({
  declarations: [...pipes],
  imports: [CommonModule],
  exports: [...pipes],
})
export class SharedModule {}
