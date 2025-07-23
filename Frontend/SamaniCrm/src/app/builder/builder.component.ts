import { moveItemInArray } from '@angular/cdk/drag-drop';
import { AfterViewInit, Component, Injector, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { BlockDefinition, BlockTypeEnum, FormTools } from './blocks/block-registry';
import { FormBuilderService } from './form-builder.service';
import { IDropEvent, transferArrayItem } from 'ngx-drag-drop-kit';
import { AppComponentBase } from '@app/app-component-base';
import { ViewModeEnum } from './models/view-mode.enum';
import { FormBuilderBackendService } from './backend.service';
import { AppConst } from '@shared/app-const';

@Component({
  standalone: false,
  selector: 'app-builder',
  templateUrl: './builder.component.html',
  styleUrls: ['./builder.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class BuilderComponent extends AppComponentBase implements OnInit, AfterViewInit, OnDestroy {
  previousMainHeaderFixedTop: boolean = AppConst.mainHeaderFixedTop;
  constructor(
    public b: FormBuilderService,
    private backendService: FormBuilderBackendService,
    injector: Injector,
  ) {
    super(injector);
    this.backendService.pageId = this.route.snapshot.params['pageId'];
  }

  ngOnInit(): void {
    this.b.cleanServiceData();
    this.b.getCustomBlocks();
    this.backendService.getPageInfo();
  }
  ngAfterViewInit(): void {
    AppConst.mainHeaderFixedTop = false;
    this.doc.querySelector('.builder-container')?.scrollIntoView();
  }
  ngOnDestroy(): void {
    AppConst.mainHeaderFixedTop = this.previousMainHeaderFixedTop;
  }

  public get ViewModeEnum(): typeof ViewModeEnum {
    return ViewModeEnum;
  }
  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  drop(event: IDropEvent<BlockDefinition[]>) {
    if (event.previousContainer.data && event.previousContainer.el.id === 'toolBox') {
      let source = event.previousContainer.data[event.previousIndex];
      this.b.addBlock(source, event.currentIndex);
    } else {
      let source = event.previousContainer.data!;
      let destination = event.container.data ?? this.b.blocks;

      transferArrayItem(source, destination, event.previousIndex, event.currentIndex);
    }
  }
}
