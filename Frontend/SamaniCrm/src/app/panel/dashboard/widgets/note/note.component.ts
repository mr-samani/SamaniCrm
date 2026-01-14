import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { WidgetNoteData } from './WidgetNoteData';
import { Widget } from '../widgets';
import { IWidgetBase } from '../IWidgetBase';
import { MatDialog } from '@angular/material/dialog';
import { EditNoteWidgetComponent } from './edit-note/edit-note-widget.component';

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.scss'],
  standalone: true,
  imports: [],
})
export class NoteComponent extends AppComponentBase implements OnInit, IWidgetBase {
  item!: Widget;
  @Input('item') set setItem(val: Widget) {
    this.item = val;
  }
  constructor(
    injector: Injector,
    private dialog: MatDialog,
  ) {
    super(injector);
  }

  ngOnInit() {}

  edit() {
    this.dialog
      .open(EditNoteWidgetComponent, {
        data: {
          item: this.item,
        },
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.item = result;
        }
      });
  }
}
