import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { HtmlEditorComponent } from '@shared/components/html-editor/html-editor.component';
import { IPageBuilderHtmlEditor } from 'ngx-page-builder';

@Injectable()
export class HtmlEditorService implements IPageBuilderHtmlEditor {
  constructor(private matDialog: MatDialog) {}
  openEditor(content: string): Promise<string> {
    return new Promise<string>((resolve, reject) => {
      this.matDialog
        .open(HtmlEditorComponent, {
          data: content,
          width: '85%',
          maxWidth: '100%',
          height: '80vh',
        })
        .afterClosed()
        .subscribe((c) => {
          if (c != undefined) {
            resolve(c);
          } else {
            reject('cancel');
          }
        });
    });
  }
}
