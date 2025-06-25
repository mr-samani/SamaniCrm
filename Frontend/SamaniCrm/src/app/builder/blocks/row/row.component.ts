import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { BlockDefinition } from '../block-registry';
import { FormBuilderService } from '@app/builder/form-builder.service';
import { DynamicRendererComponent } from '../dynamic-renderer.component';

@Component({
  selector: 'block-row',
  standalone: true,
  imports: [CommonModule, DragDropModule, DynamicRendererComponent],
  templateUrl: './row.component.html',
  styleUrl: './row.component.scss',
  providers: [FormBuilderService],
})
export class BlockRowComponent {
  @Input() index: number = 0;
  cols: { id: string }[] = [];

  childs: BlockDefinition[] = [];

  constructor(public b: FormBuilderService) {}

  @Input() set data(val: { cols: number }) {
    if (val.cols) {
      for (let i = 0; i < val.cols; i++) {
        this.cols.push({ id: `cell_${this.index}_${i}` });
      }
    }
  }

  drop(event: CdkDragDrop<BlockDefinition[]>) {
    debugger;
    if (event.previousContainer === event.container) {
      return;
    } else {
      this.b.addBlock(this.b.blocksList[event.previousIndex].type, event.currentIndex);
      // moveItemInArray(this.blocks, event.previousIndex, event.currentIndex);
    }
  }

  enter(ev: any) {
    console.log('enter', ev);
  }
}
