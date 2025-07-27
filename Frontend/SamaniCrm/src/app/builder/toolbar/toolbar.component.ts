import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { ViewModeEnum } from '../models/view-mode.enum';
import { FormBuilderService } from '../services/form-builder.service';
import { FormBuilderBackendService } from '../services/backend.service';
import { HistoryService } from '../services/history.service';

@Component({
  selector: 'toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss'],
  standalone: false,
})
export class ToolbarComponent extends AppComponentBase implements OnInit {
  constructor(
    public b: FormBuilderService,
    public history: HistoryService,
    public backendService: FormBuilderBackendService,
    injector: Injector,
  ) {
    super(injector);
  }

  ngOnInit() {}

  public get ViewModeEnum(): typeof ViewModeEnum {
    return ViewModeEnum;
  }

  get canUndo(): boolean {
    return this.history.canUndo();
  }
  get canRedo(): boolean {
    return this.history.canRedo();
  }
  undo() {
    const snapShot = this.history.undo(this.b.blocks);
    debugger;
  }
  redo() {
    const snapShot = this.history.redo(this.b.blocks);
    debugger;
  }
  getHistory() {
    console.log(this.history.getHistory());
  }
}
