import { moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { BlockDefinition, BlockTypeEnum } from './blocks/block-registry';
import { FormBuilderService } from './form-builder.service';
import { IDropEvent } from 'ngx-drag-drop-kit';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  standalone: false,
  selector: 'app-builder',
  templateUrl: './builder.component.html',
  styleUrls: ['./builder.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class BuilderComponent extends AppComponentBase implements OnInit {
  constructor(
    public b: FormBuilderService,
    injector: Injector,
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  drop(event: IDropEvent<BlockDefinition[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(this.b.blocks, event.previousIndex, event.currentIndex);
    } else {
      this.b.addBlock(this.b.blocksList[event.previousIndex].type, event.currentIndex);
    }
  }
}
