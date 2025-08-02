import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  Injector,
  OnDestroy,
  OnInit,
  ViewEncapsulation,
} from '@angular/core';
import { BlockTypeEnum } from './blocks/block-registry';
import { FormBuilderService } from './services/form-builder.service';
import { AppComponentBase } from '@app/app-component-base';
import { ViewModeEnum } from './models/view-mode.enum';
import { FormBuilderBackendService } from './services/backend.service';
import { AppConst } from '@shared/app-const';
import { HistoryService } from './services/history.service';
import { IResizableOutput } from 'ngx-drag-drop-kit';

@Component({
  standalone: false,
  selector: 'app-builder',
  templateUrl: './builder.component.html',
  styleUrls: ['./builder.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class BuilderComponent extends AppComponentBase implements OnInit, AfterViewInit, OnDestroy {
  previousMainHeaderFixedTop: boolean = AppConst.mainHeaderFixedTop;
  isRtl = AppConst.isRtl;
  rightSideWidth: number;
  propertiesWidth: number;
  constructor(
    public b: FormBuilderService,
    private backendService: FormBuilderBackendService,
    injector: Injector,
    private cd: ChangeDetectorRef,
  ) {
    super(injector);
    this.backendService.pageId = this.route.snapshot.params['pageId'];
    this.rightSideWidth = +(localStorage.getItem('builderRightSide') || 220);
    this.propertiesWidth = +(localStorage.getItem('builderProperties') || 340);
  }

  ngOnInit(): void {
    this.b.cleanServiceData();
    this.b.getCustomBlocks();
    this.backendService.getPageInfo();
  }
  ngAfterViewInit(): void {
    setTimeout(() => {
      AppConst.mainHeaderFixedTop = false;
      this.cd.detectChanges();
      this.doc.querySelector('.builder-container')?.scrollIntoView();
    });
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

  onResizeLayout(ev: IResizableOutput, type: 'rightSide' | 'properties') {
    if (type == 'rightSide') {
      localStorage.setItem('builderRightSide', ev.width.toString());
    } else {
      localStorage.setItem('builderProperties', ev.width.toString());
    }
  }
}
