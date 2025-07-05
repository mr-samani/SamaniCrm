import { moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { BlockDefinition, BlockTypeEnum, FormTools } from './blocks/block-registry';
import { FormBuilderService } from './form-builder.service';
import { IDropEvent, transferArrayItem } from 'ngx-drag-drop-kit';
import { AppComponentBase } from '@app/app-component-base';
import { ViewModeEnum } from './models/view-mode.enum';

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
  public get ViewModeEnum(): typeof ViewModeEnum {
    return ViewModeEnum;
  }
  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  drop(event: IDropEvent<BlockDefinition[]>) {
    if (event.previousContainer.data && event.previousContainer.el.id === 'toolBox') {
      let source = event.previousContainer.data[event.previousIndex];
      this.b.addBlock(source.type, event.currentIndex);
    } else {
      let source = event.previousContainer.data!;
      let destination = event.container.data ?? this.b.blocks;

      transferArrayItem(source, destination, event.previousIndex, event.currentIndex);
    }
  }
}
