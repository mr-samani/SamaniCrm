import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EditorComponent, TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';
import { HtmlEditorComponent } from './html-editor.component';
import { CommonModule } from '@angular/common';
import { TusUploadService } from '@app/file-manager/tus-upload.service';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [HtmlEditorComponent],
  imports: [CommonModule, EditorComponent, ReactiveFormsModule, FormsModule, TranslateModule],
  exports: [HtmlEditorComponent],
  providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: 'tinymce/tinymce.min.js' }, TusUploadService],
})
export class HtmlEditorModule {}
