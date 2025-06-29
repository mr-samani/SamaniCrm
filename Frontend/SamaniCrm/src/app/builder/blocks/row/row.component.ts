import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { BlockDefinition, BlockTypeEnum } from '../block-registry';
import { FormBuilderService } from '@app/builder/form-builder.service';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { BlockDivComponent } from '../div/div.component';

@Component({
  selector: 'block-row',
  standalone: true,
  imports: [CommonModule, DragDropModule, DynamicRendererComponent],
  templateUrl: './row.component.html',
  styleUrl: './row.component.scss',
})
export class BlockRowComponent implements OnInit {
  @Input() index: number = 0;

  @Input() children: BlockDefinition[] = [];

  constructor(public b: FormBuilderService) {}

  @Input() data: any;

  ngOnInit(): void {}
  drop(event: CdkDragDrop<BlockDefinition[]>, cell: BlockDefinition) {
    // اگر در همان cell جابجا شد
    if (event.previousContainer === event.container) {
      // moveItemInArray(cell.children, event.previousIndex, event.currentIndex);
      return;
    } else {
      // جابجایی بین cellها
      const item = event.previousContainer.data[event.previousIndex];
      event.previousContainer.data.splice(event.previousIndex, 1);
      event.container.data.splice(event.currentIndex, 0, item);
    }
  }

  enter(ev: any) {
    console.log('enter', ev);
  }
}
