import { Injectable, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { IPageBuilderHtmlEditor } from 'ngx-page-builder/designer';
import { HtmlEditorDialogComponent } from '../html-editor-dialog/html-editor-dialog.component';

@Injectable()
export class HtmlEditorService implements IPageBuilderHtmlEditor {
  constructor(
    private matDialog: MatDialog,
    private injector: Injector,
  ) {}
  openEditor(content: string): Promise<string> {
    return new Promise<string>((resolve, reject) => {
      this.matDialog
        .open(HtmlEditorDialogComponent, {
          data: content,
          width: '85%',
          maxWidth: '100%',
          height: '80vh',
          injector: this.injector,
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
