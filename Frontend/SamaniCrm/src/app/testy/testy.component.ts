import { Directionality } from '@angular/cdk/bidi';
import { CommonModule } from '@angular/common';
import { AfterViewInit, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { ColorSchemaService } from '@shared/services/color-schema.service';
import { IDropEvent, NgxDragDropKitModule, transferArrayItem } from 'ngx-drag-drop-kit';

@Component({
  selector: 'app-testy',
  standalone: true,
  imports: [CommonModule, MaterialCommonModule, FormsModule, NgxDragDropKitModule],
  templateUrl: './testy.component.html',
  styleUrl: './testy.component.scss',
})
export class TestyComponent implements AfterViewInit {
  items: TreeModel[] = [];
  constructor(
    private colorSchemaService: ColorSchemaService,
    private dir: Directionality,
  ) {
    this.items = [];
    for (let i = 1; i < 10; i++) {
      this.items.push({
        name: 'Item ' + i,
        children: [],
      });
    }
  }

  ngAfterViewInit(): void {}

  add() {
    let rndPosition = Math.floor(Math.random() * this.items.length);
    let rndName = 'added item_' + Math.round(Math.random() * 9999);
    this.items.splice(rndPosition, 0, { name: rndName, children: [] });
  }

  drop(event: IDropEvent) {
    transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
  }
}
export interface TreeModel {
  name: string;
  children: TreeModel[];
}
