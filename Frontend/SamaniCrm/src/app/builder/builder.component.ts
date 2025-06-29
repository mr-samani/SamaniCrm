import { DragDropModule, CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { BLOCK_REGISTRY, BlockDefinition, BlockTypeEnum } from './blocks/block-registry';
import { DynamicRendererComponent } from './blocks/dynamic-renderer.component';
import { TranslateModule } from '@ngx-translate/core';
import { FormBuilderService } from './form-builder.service';
import { BlockRowComponent } from './blocks/row/row.component';

@Component({
  selector: 'app-builder',
  templateUrl: './builder.component.html',
  styleUrls: ['./builder.component.scss'],
  encapsulation: ViewEncapsulation.None,
  imports: [CommonModule, DragDropModule, DynamicRendererComponent, TranslateModule, BlockRowComponent],
  providers: [FormBuilderService],
})
export class BuilderComponent implements OnInit {
  constructor(public b: FormBuilderService) {}

  ngOnInit(): void {
  }

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  drop(event: CdkDragDrop<BlockDefinition[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(this.b.blocks, event.previousIndex, event.currentIndex);
    } else {
      this.b.addBlock(this.b.blocksList[event.previousIndex].type, event.currentIndex);
    }
  }
}
