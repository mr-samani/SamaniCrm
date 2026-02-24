import { CommonModule } from '@angular/common';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { HtmlEditorModule } from '@shared/components/html-editor/html-editor.module';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { SharedModule } from '@shared/shared.module';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-html-editor-dialog',
  templateUrl: './html-editor-dialog.component.html',
  styleUrls: ['./html-editor-dialog.component.scss'],
  standalone: true,
  imports: [CommonModule, HtmlEditorModule, MatDialogModule, SharedModule, FormsModule],
})
export class HtmlEditorDialogComponent extends AppComponentBase implements OnInit {
  model: string;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) data: string,
    private dialogRef: MatDialogRef<HtmlEditorDialogComponent>,
  ) {
    super();
    this.model = data;
  }

  ngOnInit() {}

  ok() {
    this.dialogRef.close(this.model);
  }
}
