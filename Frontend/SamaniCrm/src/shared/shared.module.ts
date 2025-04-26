import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SanitizerPipe } from './pipes/sanitizer.pipe';
import { LuxonFormatPipe } from './pipes/luxon-format.pipe';
import { LuxonFromNowPipe } from './pipes/luxon-from-now.pipe';
import { FileSizePipe } from './pipes/file-size.pipe';

@NgModule({
  declarations: [SanitizerPipe, LuxonFormatPipe, LuxonFromNowPipe, FileSizePipe],
  imports: [CommonModule],
  exports: [SanitizerPipe, LuxonFormatPipe, LuxonFromNowPipe, FileSizePipe],
})
export class SharedModule {}
